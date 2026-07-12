using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Ordering.Commands;

public record StartPreparingOrderCommand(Guid OrderId) : IRequest;
public record MarkOrderReadyCommand(Guid OrderId) : IRequest;
public record CompleteOrderCommand(Guid OrderId) : IRequest;
public record CancelOrderCommand(Guid OrderId) : IRequest;

public class StartPreparingOrderCommandHandler(IApplicationDbContext context) : IRequestHandler<StartPreparingOrderCommand>
{
    public async Task Handle(StartPreparingOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Ordering.Order), request.OrderId);

        order.StartPreparing();
        await context.SaveChangesAsync(cancellationToken);
    }
}

public class MarkOrderReadyCommandHandler(IApplicationDbContext context) : IRequestHandler<MarkOrderReadyCommand>
{
    public async Task Handle(MarkOrderReadyCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Ordering.Order), request.OrderId);

        order.MarkReadyForDeliveryOrPickup();
        await context.SaveChangesAsync(cancellationToken);
    }
}

public class CompleteOrderCommandHandler(IApplicationDbContext context) : IRequestHandler<CompleteOrderCommand>
{
    public async Task Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Ordering.Order), request.OrderId);

        order.Complete();
        await context.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>Cancelar repõe o estoque de todos os itens do pedido, independentemente do estado do pagamento.</summary>
public class CancelOrderCommandHandler(IApplicationDbContext context) : IRequestHandler<CancelOrderCommand>
{
    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Ordering.Order), request.OrderId);

        var productIds = order.Items.Select(i => i.ProductId).ToList();
        var products = await context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        order.Cancel();

        foreach (var item in order.Items)
        {
            if (products.TryGetValue(item.ProductId, out var product))
                product.IncreaseStock(item.Quantity);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
