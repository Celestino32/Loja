using LojaVirtual.Domain.Invoicing;

namespace LojaVirtual.Application.Invoicing;

/// <summary>
/// Gera o PDF de uma fatura já emitida. Chamado tanto na emissão original quanto na
/// "segunda via" — os dados são sempre os mesmos (a fatura é imutável), então o PDF é
/// idêntico em ambos os casos; só o rótulo pode indicar que se trata de uma reimpressão.
/// </summary>
public interface IInvoicePdfGenerator
{
    byte[] Generate(Invoice invoice, IReadOnlyList<InvoiceLineItem> items, bool isSecondCopy);
}

public record InvoiceLineItem(string ProductName, decimal UnitPrice, int Quantity)
{
    public decimal LineTotal => UnitPrice * Quantity;
}
