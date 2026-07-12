using FluentValidation;
using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Common;
using LojaVirtual.Domain.Identity;
using LojaVirtual.Domain.Ordering;
using LojaVirtual.Domain.Payments;
using LojaVirtual.Domain.Shipping;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Ordering.Commands;

public record CheckoutItem(Guid ProductId, int Quantity);

public record CheckoutCommand(
    Guid UserId,
    List<CheckoutItem> Items,
    FulfillmentType FulfillmentType,
    Guid? ShippingZoneId,
    string? ShippingStreet,
    string? ShippingReference,
    PaymentMethod PaymentMethod,
    string? CouponCode = null) : IRequest<CheckoutResultDto>;

public record CheckoutResultDto(
    Guid OrderId,
    string OrderNumber,
    decimal Total,
    decimal DiscountTotal,
    PaymentMethod PaymentMethod,
    string? PaymentExternalReference,
    string PaymentInstructions);

public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    public CheckoutCommandValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("O carrinho está vazio.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });

        RuleFor(x => x.ShippingZoneId).NotNull().When(x => x.FulfillmentType == FulfillmentType.Entrega)
            .WithMessage("Selecione uma zona de entrega.");
        RuleFor(x => x.ShippingStreet).NotEmpty().When(x => x.FulfillmentType == FulfillmentType.Entrega)
            .WithMessage("Informe o endereço de entrega.");
    }
}

public class CheckoutCommandHandler(IApplicationDbContext context, IEnumerable<IPaymentGateway> paymentGateways)
    : IRequestHandler<CheckoutCommand, CheckoutResultDto>
{
    public async Task<CheckoutResultDto> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), request.UserId);

        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        foreach (var item in request.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
                throw new NotFoundException(nameof(Domain.Catalog.Product), item.ProductId);
            if (!product.IsActive)
                throw new DomainException($"O produto '{product.Name}' não está disponível.");
            if (product.StockQuantity < item.Quantity)
                throw new DomainException($"Estoque insuficiente para '{product.Name}'. Disponível: {product.StockQuantity}.");
        }

        ShippingZone? zone = null;
        if (request.FulfillmentType == FulfillmentType.Entrega)
        {
            zone = await context.ShippingZones.FirstOrDefaultAsync(z => z.Id == request.ShippingZoneId, cancellationToken)
                ?? throw new NotFoundException(nameof(ShippingZone), request.ShippingZoneId!);
            if (!zone.IsActive)
                throw new DomainException("A zona de entrega selecionada não está disponível.");
        }

        var order = new Order(
            orderNumber: GenerateOrderNumber(),
            customerId: customer.Id,
            fulfillmentType: request.FulfillmentType,
            shippingCost: zone?.Cost ?? 0,
            shippingProvince: zone?.Province,
            shippingMunicipality: zone?.Municipality,
            shippingStreet: request.ShippingStreet,
            shippingReference: request.ShippingReference);

        foreach (var item in request.Items)
        {
            var product = products[item.ProductId];
            order.AddItem(product.Id, product.Name, product.Price, item.Quantity);
        }

        Domain.Coupons.Coupon? coupon = null;
        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var normalizedCode = request.CouponCode.Trim().ToUpperInvariant();
            coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken)
                ?? throw new DomainException("Cupom inválido.");

            var itemsSubtotal = order.Items.Sum(i => i.LineTotal);
            coupon.EnsureRedeemable(itemsSubtotal, DateTime.UtcNow);

            var discount = coupon.CalculateDiscount(itemsSubtotal);
            order.ApplyDiscount(discount, coupon.Code);
        }

        // O gateway é chamado antes de qualquer gravação em BD: se falhar, nada é persistido
        // (sem estoque reservado nem pedido "fantasma" para um pagamento que não foi iniciado).
        var gateway = paymentGateways.FirstOrDefault(g => g.Method == request.PaymentMethod)
            ?? throw new DomainException("Método de pagamento não suportado.");
        var paymentResult = await gateway.InitiateAsync(order.Id, order.OrderNumber, order.Total, cancellationToken);

        foreach (var item in request.Items)
            products[item.ProductId].DecreaseStock(item.Quantity);

        var payment = new Payment(order.Id, request.PaymentMethod, order.Total, paymentResult.ExternalReference, paymentResult.Instructions);

        // Não decrementamos o uso do cupom caso o pedido seja cancelado depois — reversão de
        // uso de cupom é uma decisão manual, para evitar abuso de cancela-e-recompra.
        coupon?.RegisterUse();

        context.Orders.Add(order);
        context.Payments.Add(payment);
        await context.SaveChangesAsync(cancellationToken);

        return new CheckoutResultDto(
            order.Id,
            order.OrderNumber,
            order.Total,
            order.DiscountTotal,
            request.PaymentMethod,
            paymentResult.ExternalReference,
            paymentResult.Instructions);
    }

    private static string GenerateOrderNumber()
        => $"PED-{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}";
}
