using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Application.Common.Settings;
using LojaVirtual.Domain.Invoicing;
using LojaVirtual.Domain.Ordering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LojaVirtual.Application.Invoicing;

public class InvoiceIssuer(IApplicationDbContext context, IOptions<CompanySettings> companySettings) : IInvoiceIssuer
{
    private readonly CompanySettings _company = companySettings.Value;

    public async Task<Invoice> IssueForOrderAsync(Order order, CancellationToken cancellationToken)
    {
        var existing = await context.Invoices.FirstOrDefaultAsync(i => i.OrderId == order.Id, cancellationToken);
        if (existing is not null)
            return existing;

        var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == order.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Identity.Customer), order.CustomerId);

        var currentYear = DateTime.UtcNow.Year;
        var seriesCode = $"FT {currentYear}";

        // Numeração sequencial sem furos: reserva e grava dentro da mesma transação; sob a
        // rara colisão de concorrência (constraint única SeriesCode+SequenceNumber), tenta
        // novamente com o próximo número.
        const int maxAttempts = 3;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var series = await context.InvoiceSeries.FirstOrDefaultAsync(s => s.Code == seriesCode, cancellationToken);
            if (series is null)
            {
                series = new InvoiceSeries(seriesCode);
                context.InvoiceSeries.Add(series);
            }

            var sequenceNumber = series.ReserveNextSequenceNumber();

            var invoice = new Invoice(
                invoiceSeriesId: series.Id,
                seriesCode: series.Code,
                sequenceNumber: sequenceNumber,
                orderId: order.Id,
                sellerName: _company.Name,
                sellerNif: _company.Nif,
                customerName: customer.FullName,
                customerNif: customer.Nif,
                subtotal: order.Total);

            context.Invoices.Add(invoice);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return invoice;
            }
            catch (DbUpdateException) when (attempt < maxAttempts)
            {
                context.Invoices.Remove(invoice);
            }
        }

        throw new InvalidOperationException("Não foi possível emitir a fatura após múltiplas tentativas. Tente novamente.");
    }
}
