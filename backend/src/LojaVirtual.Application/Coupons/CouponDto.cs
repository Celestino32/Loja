using LojaVirtual.Domain.Coupons;

namespace LojaVirtual.Application.Coupons;

public record CouponDto(
    Guid Id,
    string Code,
    CouponDiscountType DiscountType,
    decimal DiscountValue,
    decimal? MinOrderValue,
    int? MaxUses,
    int UsedCount,
    DateTime? ValidFrom,
    DateTime? ValidUntil,
    bool IsActive);
