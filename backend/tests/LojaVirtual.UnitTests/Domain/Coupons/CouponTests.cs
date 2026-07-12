using LojaVirtual.Domain.Common;
using LojaVirtual.Domain.Coupons;

namespace LojaVirtual.UnitTests.Domain.Coupons;

public class CouponTests
{
    [Fact]
    public void CalculateDiscount_Percentual_ComputesCorrectAmount()
    {
        var coupon = new Coupon("PROMO10", CouponDiscountType.Percentual, 10m, null, null, null, null);

        Assert.Equal(10m, coupon.CalculateDiscount(100m));
    }

    [Fact]
    public void CalculateDiscount_ValorFixo_NeverExceedsSubtotal()
    {
        var coupon = new Coupon("DESC50000", CouponDiscountType.ValorFixo, 50000m, null, null, null, null);

        Assert.Equal(30000m, coupon.CalculateDiscount(30000m));
    }

    [Fact]
    public void Constructor_PercentualAbove100_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new Coupon("X", CouponDiscountType.Percentual, 150m, null, null, null, null));
    }

    [Fact]
    public void EnsureRedeemable_Inactive_ThrowsDomainException()
    {
        var coupon = new Coupon("PROMO10", CouponDiscountType.Percentual, 10m, null, null, null, null);
        coupon.Deactivate();

        Assert.Throws<DomainException>(() => coupon.EnsureRedeemable(100m, DateTime.UtcNow));
    }

    [Fact]
    public void EnsureRedeemable_Expired_ThrowsDomainException()
    {
        var coupon = new Coupon("PROMO10", CouponDiscountType.Percentual, 10m, null, null, null, DateTime.UtcNow.AddDays(-1));

        Assert.Throws<DomainException>(() => coupon.EnsureRedeemable(100m, DateTime.UtcNow));
    }

    [Fact]
    public void EnsureRedeemable_BelowMinOrderValue_ThrowsDomainException()
    {
        var coupon = new Coupon("PROMO10", CouponDiscountType.Percentual, 10m, 50000m, null, null, null);

        Assert.Throws<DomainException>(() => coupon.EnsureRedeemable(10000m, DateTime.UtcNow));
    }

    [Fact]
    public void EnsureRedeemable_MaxUsesReached_ThrowsDomainException()
    {
        var coupon = new Coupon("PROMO10", CouponDiscountType.Percentual, 10m, null, 1, null, null);
        coupon.RegisterUse();

        Assert.Throws<DomainException>(() => coupon.EnsureRedeemable(100m, DateTime.UtcNow));
    }

    [Fact]
    public void EnsureRedeemable_ValidCoupon_DoesNotThrow()
    {
        var coupon = new Coupon("PROMO10", CouponDiscountType.Percentual, 10m, 10000m, 5, null, null);

        var exception = Record.Exception(() => coupon.EnsureRedeemable(50000m, DateTime.UtcNow));

        Assert.Null(exception);
    }
}
