using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Application.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LojaVirtual.Application.Invoicing;

public record GetSaftExportDataQuery(DateOnly From, DateOnly To) : IRequest<SaftExportData>;

public class GetSaftExportDataQueryHandler(IApplicationDbContext context, IOptions<CompanySettings> companySettings)
    : IRequestHandler<GetSaftExportDataQuery, SaftExportData>
{
    public async Task<SaftExportData> Handle(GetSaftExportDataQuery request, CancellationToken cancellationToken)
    {
        var fromUtc = request.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toUtc = request.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var invoices = await context.Invoices.AsNoTracking()
            .Where(i => i.IssuedAtUtc >= fromUtc && i.IssuedAtUtc <= toUtc)
            .OrderBy(i => i.SequenceNumber)
            .ToListAsync(cancellationToken);

        var orderIds = invoices.Select(i => i.OrderId).ToList();
        var ordersWithItems = await context.Orders.AsNoTracking()
            .Include(o => o.Items)
            .Where(o => orderIds.Contains(o.Id))
            .ToDictionaryAsync(o => o.Id, cancellationToken);

        var exportInvoices = invoices.Select(invoice =>
        {
            var lines = ordersWithItems.TryGetValue(invoice.OrderId, out var order)
                ? order.Items.Select(i => new SaftExportInvoiceLine(i.ProductName, i.Quantity, i.UnitPrice, i.LineTotal)).ToList()
                : [];

            return new SaftExportInvoice(
                invoice.FullNumber,
                invoice.IssuedAtUtc,
                invoice.CustomerName,
                invoice.CustomerNif,
                invoice.Subtotal,
                invoice.VatRate,
                invoice.VatAmount,
                invoice.Total,
                invoice.IsCancelled,
                lines);
        }).ToList();

        var company = companySettings.Value;
        return new SaftExportData(company.Name, company.Nif, request.From, request.To, exportInvoices);
    }
}
