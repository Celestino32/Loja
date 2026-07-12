using LojaVirtual.Domain.Common;
using LojaVirtual.Domain.Invoicing;

namespace LojaVirtual.UnitTests.Domain.Invoicing;

public class InvoiceTests
{
    [Fact]
    public void Constructor_CalculatesVatAt14Percent()
    {
        var invoice = new Invoice(
            invoiceSeriesId: Guid.NewGuid(),
            seriesCode: "FT 2026",
            sequenceNumber: 1,
            orderId: Guid.NewGuid(),
            sellerName: "Loja Virtual, Lda",
            sellerNif: "5000000000",
            customerName: "Cliente Teste",
            customerNif: "123456789",
            subtotal: 100000m);

        Assert.Equal(14000m, invoice.VatAmount);
        Assert.Equal(114000m, invoice.Total);
    }

    [Fact]
    public void FullNumber_CombinesSeriesCodeAndPaddedSequence()
    {
        var invoice = new Invoice(
            Guid.NewGuid(), "FT 2026", 42, Guid.NewGuid(),
            "Loja Virtual, Lda", "5000000000", "Cliente Teste", null, 1000m);

        Assert.Equal("FT 2026/00042", invoice.FullNumber);
    }

    [Fact]
    public void Cancel_Twice_ThrowsDomainException()
    {
        var invoice = new Invoice(
            Guid.NewGuid(), "FT 2026", 1, Guid.NewGuid(),
            "Loja Virtual, Lda", "5000000000", "Cliente Teste", null, 1000m);

        invoice.Cancel();

        Assert.Throws<DomainException>(invoice.Cancel);
    }

    [Fact]
    public void ReserveNextSequenceNumber_IncrementsSequentially()
    {
        var series = new InvoiceSeries("FT 2026");

        var first = series.ReserveNextSequenceNumber();
        var second = series.ReserveNextSequenceNumber();

        Assert.Equal(1, first);
        Assert.Equal(2, second);
    }
}
