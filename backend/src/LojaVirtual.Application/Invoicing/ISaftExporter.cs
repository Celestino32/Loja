namespace LojaVirtual.Application.Invoicing;

public record SaftExportInvoiceLine(string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);

public record SaftExportInvoice(
    string FullNumber,
    DateTime IssuedAtUtc,
    string CustomerName,
    string? CustomerNif,
    decimal Subtotal,
    decimal VatRate,
    decimal VatAmount,
    decimal Total,
    bool IsCancelled,
    List<SaftExportInvoiceLine> Lines);

public record SaftExportData(
    string CompanyName,
    string CompanyNif,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    List<SaftExportInvoice> Invoices);

/// <summary>
/// Gera o XML de exportação no formato SAF-T (AO). A estrutura segue o padrão OCDE/SAF-T
/// amplamente adotado por administrações fiscais lusófonas, mas NÃO foi validada contra o XSD
/// oficial da AGT (não disponível publicamente) — ver comentário em <c>XmlSaftExporter</c>.
/// </summary>
public interface ISaftExporter
{
    byte[] GenerateXml(SaftExportData data);
}
