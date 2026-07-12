using LojaVirtual.Domain.Common;
using LojaVirtual.Domain.Payments;

namespace LojaVirtual.UnitTests.Domain.Payments;

public class PaymentTests
{
    [Fact]
    public void Constructor_WithZeroAmount_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Payment(Guid.NewGuid(), PaymentMethod.ReferenciaDePagamento, 0m, null, "instruções"));
    }

    [Fact]
    public void Confirm_SetsStatusAndConfirmedAt()
    {
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.TransferenciaBancaria, 1000m, null, "instruções");

        payment.Confirm("REF-123");

        Assert.Equal(PaymentStatus.Confirmado, payment.Status);
        Assert.Equal("REF-123", payment.ExternalReference);
        Assert.NotNull(payment.ConfirmedAtUtc);
    }

    [Fact]
    public void Confirm_Twice_ThrowsDomainException()
    {
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.TransferenciaBancaria, 1000m, null, "instruções");
        payment.Confirm();

        Assert.Throws<DomainException>(() => payment.Confirm());
    }

    [Fact]
    public void Fail_AfterConfirmed_ThrowsDomainException()
    {
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.TransferenciaBancaria, 1000m, null, "instruções");
        payment.Confirm();

        Assert.Throws<DomainException>(payment.Fail);
    }

    [Fact]
    public void Confirm_AfterFailed_ThrowsDomainException()
    {
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.TransferenciaBancaria, 1000m, null, "instruções");
        payment.Fail();

        Assert.Throws<DomainException>(() => payment.Confirm());
    }
}
