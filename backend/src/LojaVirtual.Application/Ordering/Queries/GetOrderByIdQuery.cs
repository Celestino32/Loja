using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Ordering.Queries;

/// <summary>
/// Quando <paramref name="RequestingUserId"/> é informado (cliente autenticado), só retorna o
/// pedido se ele pertencer a esse cliente — caso contrário trata como não encontrado, sem
/// revelar a existência do pedido de outra pessoa. Staff (RequestingUserId nulo) vê qualquer pedido.
/// </summary>
public record GetOrderByIdQuery(Guid OrderId, Guid? RequestingUserId) : IRequest<OrderDto>;

public class GetOrderByIdQueryHandler(IApplicationDbContext context) : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await context.Orders.AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Ordering.Order), request.OrderId);

        var orderCustomer = await context.Customers.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == order.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Identity.Customer), order.CustomerId);

        if (request.RequestingUserId is { } userId && orderCustomer.UserId != userId)
            throw new NotFoundException(nameof(Domain.Ordering.Order), request.OrderId);

        var hasInvoice = await context.Invoices.AsNoTracking().AnyAsync(i => i.OrderId == order.Id, cancellationToken);

        return MapToDto(order, hasInvoice, orderCustomer.FullName, orderCustomer.Phone);
    }

    internal static OrderDto MapToDto(Domain.Ordering.Order order, bool hasInvoice, string customerName, string customerPhone) => new(
        order.Id,
        order.OrderNumber,
        order.Status,
        order.FulfillmentType,
        order.ShippingProvince,
        order.ShippingMunicipality,
        order.ShippingStreet,
        order.ShippingReference,
        order.ShippingCost,
        order.DiscountTotal,
        order.Total,
        order.CreatedAtUtc,
        order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal)).ToList(),
        order.Payment is null
            ? null
            : new PaymentDto(order.Payment.Method, order.Payment.Status, order.Payment.Amount, order.Payment.ExternalReference, order.Payment.Instructions, order.Payment.ConfirmedAtUtc),
        hasInvoice,
        customerName,
        customerPhone);
}
