using LojaVirtual.Application.Invoicing;
using LojaVirtual.Domain.Invoicing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LojaVirtual.Infrastructure.Invoicing;

public class QuestPdfInvoiceGenerator : IInvoicePdfGenerator
{
    public byte[] Generate(Invoice invoice, IReadOnlyList<InvoiceLineItem> items, bool isSecondCopy)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(invoice.SellerName).Bold().FontSize(14);
                            c.Item().Text($"NIF: {invoice.SellerNif}");
                        });

                        row.ConstantItem(180).Column(c =>
                        {
                            c.Item().AlignRight().Text("FATURA").Bold().FontSize(16);
                            c.Item().AlignRight().Text(invoice.FullNumber).FontSize(12);
                            if (isSecondCopy)
                                c.Item().AlignRight().Text("(2ª via)").Italic();
                            c.Item().AlignRight().Text($"Emitida em: {invoice.IssuedAtUtc:dd/MM/yyyy HH:mm}");
                        });
                    });

                    column.Item().PaddingTop(10).LineHorizontal(1);
                });

                page.Content().PaddingVertical(15).Column(column =>
                {
                    column.Spacing(15);

                    column.Item().Column(c =>
                    {
                        c.Item().Text("Cliente").Bold();
                        c.Item().Text(invoice.CustomerName);
                        if (!string.IsNullOrWhiteSpace(invoice.CustomerNif))
                            c.Item().Text($"NIF: {invoice.CustomerNif}");
                    });

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Produto").Bold();
                            header.Cell().AlignRight().Text("Qtd.").Bold();
                            header.Cell().AlignRight().Text("Preço unit.").Bold();
                            header.Cell().AlignRight().Text("Total").Bold();
                            header.Cell().ColumnSpan(4).PaddingTop(3).BorderBottom(1);
                        });

                        foreach (var item in items)
                        {
                            table.Cell().PaddingVertical(3).Text(item.ProductName);
                            table.Cell().PaddingVertical(3).AlignRight().Text(item.Quantity.ToString());
                            table.Cell().PaddingVertical(3).AlignRight().Text($"{item.UnitPrice:N2} Kz");
                            table.Cell().PaddingVertical(3).AlignRight().Text($"{item.LineTotal:N2} Kz");
                        }
                    });

                    column.Item().AlignRight().Column(c =>
                    {
                        c.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text("Subtotal:");
                            row.ConstantItem(120).AlignRight().Text($"{invoice.Subtotal:N2} Kz");
                        });
                        c.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text($"IVA ({invoice.VatRate:P0}):");
                            row.ConstantItem(120).AlignRight().Text($"{invoice.VatAmount:N2} Kz");
                        });
                        c.Item().PaddingTop(3).BorderTop(1).Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text("Total:").Bold();
                            row.ConstantItem(120).AlignRight().Text($"{invoice.Total:N2} Kz").Bold();
                        });
                    });

                    if (invoice.IsCancelled)
                        column.Item().AlignCenter().Text("FATURA ANULADA").Bold().FontColor(Colors.Red.Medium);
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Documento sem valor fiscal certificado pela AGT — emitido enquanto a certificação do software está em curso.")
                        .FontSize(8).Italic();
                });
            });
        });

        return document.GeneratePdf();
    }
}
