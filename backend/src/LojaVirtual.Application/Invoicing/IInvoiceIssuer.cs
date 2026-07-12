using LojaVirtual.Domain.Invoicing;
using LojaVirtual.Domain.Ordering;

namespace LojaVirtual.Application.Invoicing;

/// <summary>
/// Emite a fatura fiscal-ready de um pedido pago. Garante numeração sequencial sem furos
/// (via <see cref="InvoiceSeries.ReserveNextSequenceNumber"/>) e nunca emite duas faturas
/// para o mesmo pedido.
/// </summary>
public interface IInvoiceIssuer
{
    Task<Invoice> IssueForOrderAsync(Order order, CancellationToken cancellationToken);
}
