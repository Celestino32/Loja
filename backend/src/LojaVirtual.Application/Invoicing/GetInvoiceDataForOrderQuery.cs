using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Invoicing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Invoicing;

public record InvoicePdfData(Invoice Invoice, List<InvoiceLineItem> Items);

/// <summary>Mesma regra de autorização de <c>GetOrderByIdQuery</c>: cliente só vê a própria fatura.</summary>
public record GetInvoiceDataForOrderQuery(Guid OrderId, Guid? RequestingUserId) : IRequest<InvoicePdfData>;

public class GetInvoiceDataForOrderQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetInvoiceDataForOrderQuery, InvoicePdfData>
{
    public async Task<InvoicePdfData> Handle(GetInvoiceDataForOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await context.Orders.AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Ordering.Order), request.OrderId);

        if (request.RequestingUserId is { } userId)
        {
            var customer = await context.Customers.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == order.CustomerId, cancellationToken);
            if (customer is null || customer.UserId != userId)
                throw new NotFoundException(nameof(Domain.Ordering.Order), request.OrderId);
        }

        var invoice = await context.Invoices
            .FirstOrDefaultAsync(i => i.OrderId == order.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Invoice), request.OrderId);

        var items = order.Items
            .Select(i => new InvoiceLineItem(i.ProductName, i.UnitPrice, i.Quantity))
            .ToList();

        return new InvoicePdfData(invoice, items);
    }
}
