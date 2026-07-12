using System.Globalization;
using System.Xml.Linq;
using LojaVirtual.Application.Invoicing;

namespace LojaVirtual.Infrastructure.Invoicing;

/// <summary>
/// Gera um XML no formato geral SAF-T (Standard Audit File for Tax), seguindo a estrutura
/// OCDE amplamente usada como base pelas administrações fiscais lusófonas (Header,
/// MasterFiles, SourceDocuments/SalesInvoices).
///
/// ATENÇÃO — leia antes de usar em produção:
/// 1. Este arquivo NÃO foi validado contra o XSD oficial do SAF-T (AO) publicado pela AGT
///    (Administração Geral Tributária de Angola), que não está publicamente disponível para
///    verificação automatizada. A estrutura aqui é uma estimativa de boa-fé baseada no padrão
///    conhecido; campos, nomes ou obrigatoriedades podem divergir da versão oficial.
/// 2. O campo &lt;Hash&gt; de cada fatura é OBRIGATÓRIO no SAF-T real e deve ser uma assinatura
///    digital gerada com uma chave privada emitida pela AGT durante a certificação do software
///    de faturação — não temos essa chave, então o campo é preenchido com um marcador
///    "CERTIFICACAO_PENDENTE" em vez de simular uma assinatura válida.
/// 3. Antes de submeter este arquivo à AGT, é necessário concluir o processo de certificação
///    do software e validar a estrutura exata exigida.
/// </summary>
public class XmlSaftExporter : ISaftExporter
{
    private const string PlaceholderHash = "CERTIFICACAO_PENDENTE";
    private static readonly XNamespace Ns = "urn:OECD:StandardAuditFile-Tax:AO_1.01_01";

    public byte[] GenerateXml(SaftExportData data)
    {
        var invoicesWithLines = data.Invoices.Where(i => !i.IsCancelled).ToList();
        var totalCredit = invoicesWithLines.Sum(i => i.Total);

        var document = new XElement(Ns + "AuditFile",
            BuildHeader(data),
            BuildMasterFiles(data),
            BuildSourceDocuments(data, invoicesWithLines, totalCredit));

        using var stream = new MemoryStream();
        var xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", null), document);
        xDocument.Save(stream);
        return stream.ToArray();
    }

    private static XElement BuildHeader(SaftExportData data) =>
        new(Ns + "Header",
            new XElement(Ns + "AuditFileVersion", "1.01_01"),
            new XElement(Ns + "CompanyName", data.CompanyName),
            new XElement(Ns + "TaxRegistrationNumber", data.CompanyNif),
            new XElement(Ns + "TaxAccountingBasis", "F"),
            new XElement(Ns + "StartDate", data.PeriodStart.ToString("yyyy-MM-dd")),
            new XElement(Ns + "EndDate", data.PeriodEnd.ToString("yyyy-MM-dd")),
            new XElement(Ns + "CurrencyCode", "AOA"),
            new XElement(Ns + "DateCreated", DateTime.UtcNow.ToString("yyyy-MM-dd")),
            new XElement(Ns + "ProductID", "LojaVirtual/1.0"),
            new XElement(Ns + "ProductVersion", "1.0"),
            new XElement(Ns + "SoftwareCertificateNumber", "0"));

    private static XElement BuildMasterFiles(SaftExportData data)
    {
        var customers = data.Invoices
            .Select(i => new { i.CustomerName, i.CustomerNif })
            .Distinct()
            .Select((c, index) => new XElement(Ns + "Customer",
                new XElement(Ns + "CustomerID", (index + 1).ToString(CultureInfo.InvariantCulture)),
                new XElement(Ns + "CustomerTaxID", c.CustomerNif ?? "999999999"),
                new XElement(Ns + "CompanyName", c.CustomerName)));

        return new XElement(Ns + "MasterFiles",
            customers,
            new XElement(Ns + "TaxTable",
                new XElement(Ns + "TaxTableEntry",
                    new XElement(Ns + "TaxType", "IVA"),
                    new XElement(Ns + "TaxCountryRegion", "AO"),
                    new XElement(Ns + "TaxCode", "NOR"),
                    new XElement(Ns + "Description", "Taxa normal"),
                    new XElement(Ns + "TaxPercentage", "14.00"))));
    }

    private static XElement BuildSourceDocuments(SaftExportData data, List<SaftExportInvoice> invoicesWithLines, decimal totalCredit)
    {
        var customerIds = data.Invoices
            .Select(i => new { i.CustomerName, i.CustomerNif })
            .Distinct()
            .Select((c, index) => (c.CustomerName, c.CustomerNif, Id: index + 1))
            .ToList();

        int GetCustomerId(string name, string? nif) =>
            customerIds.First(c => c.CustomerName == name && c.CustomerNif == nif).Id;

        var invoiceElements = data.Invoices.Select(invoice =>
        {
            var lineElements = invoice.Lines.Select((line, index) =>
                new XElement(Ns + "Line",
                    new XElement(Ns + "LineNumber", (index + 1).ToString(CultureInfo.InvariantCulture)),
                    new XElement(Ns + "ProductDescription", line.ProductName),
                    new XElement(Ns + "Quantity", line.Quantity.ToString(CultureInfo.InvariantCulture)),
                    new XElement(Ns + "UnitPrice", line.UnitPrice.ToString("F2", CultureInfo.InvariantCulture)),
                    new XElement(Ns + "CreditAmount", line.LineTotal.ToString("F2", CultureInfo.InvariantCulture)),
                    new XElement(Ns + "Tax",
                        new XElement(Ns + "TaxType", "IVA"),
                        new XElement(Ns + "TaxCountryRegion", "AO"),
                        new XElement(Ns + "TaxCode", "NOR"),
                        new XElement(Ns + "TaxPercentage", "14.00"))));

            return new XElement(Ns + "Invoice",
                new XElement(Ns + "InvoiceNo", invoice.FullNumber),
                new XElement(Ns + "DocumentStatus",
                    new XElement(Ns + "InvoiceStatus", invoice.IsCancelled ? "A" : "N"),
                    new XElement(Ns + "InvoiceStatusDate", invoice.IssuedAtUtc.ToString("yyyy-MM-ddTHH:mm:ss"))),
                new XElement(Ns + "Hash", PlaceholderHash),
                new XElement(Ns + "InvoiceDate", invoice.IssuedAtUtc.ToString("yyyy-MM-dd")),
                new XElement(Ns + "InvoiceType", "FT"),
                new XElement(Ns + "SystemEntryDate", invoice.IssuedAtUtc.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement(Ns + "CustomerID", GetCustomerId(invoice.CustomerName, invoice.CustomerNif).ToString(CultureInfo.InvariantCulture)),
                lineElements,
                new XElement(Ns + "DocumentTotals",
                    new XElement(Ns + "TaxPayable", invoice.VatAmount.ToString("F2", CultureInfo.InvariantCulture)),
                    new XElement(Ns + "NetTotal", invoice.Subtotal.ToString("F2", CultureInfo.InvariantCulture)),
                    new XElement(Ns + "GrossTotal", invoice.Total.ToString("F2", CultureInfo.InvariantCulture))));
        });

        return new XElement(Ns + "SourceDocuments",
            new XElement(Ns + "SalesInvoices",
                new XElement(Ns + "NumberOfEntries", invoicesWithLines.Count.ToString(CultureInfo.InvariantCulture)),
                new XElement(Ns + "TotalDebit", "0.00"),
                new XElement(Ns + "TotalCredit", totalCredit.ToString("F2", CultureInfo.InvariantCulture)),
                invoiceElements));
    }
}
