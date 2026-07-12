using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Coupons;

/// <summary>
/// Cupom de desconto aplicável no checkout. O desconto nunca ultrapassa o subtotal do pedido
/// (não gera total negativo) e a validação completa (validade, limite de uso, valor mínimo)
/// acontece em <see cref="EnsureRedeemable"/>, chamada pelo handler de checkout antes de aplicar.
/// </summary>
public class Coupon : AuditableEntity
{
    public string Code { get; private set; } = default!;
    public CouponDiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public decimal? MinOrderValue { get; private set; }
    public int? MaxUses { get; private set; }
    public int UsedCount { get; private set; }
    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidUntil { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Coupon()
    {
    }

    public Coupon(
        string code,
        CouponDiscountType discountType,
        decimal discountValue,
        decimal? minOrderValue,
        int? maxUses,
        DateTime? validFrom,
        DateTime? validUntil)
    {
        SetCode(code);
        SetDiscount(discountType, discountValue);

        if (minOrderValue is < 0)
            throw new DomainException("O valor mínimo do pedido não pode ser negativo.");
        if (maxUses is <= 0)
            throw new DomainException("O limite de utilizações deve ser maior que zero.");
        if (validFrom.HasValue && validUntil.HasValue && validFrom > validUntil)
            throw new DomainException("A data de início não pode ser depois da data de fim.");

        MinOrderValue = minOrderValue;
        MaxUses = maxUses;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
    }

    public decimal CalculateDiscount(decimal orderSubtotal)
    {
        var discount = DiscountType == CouponDiscountType.Percentual
            ? Math.Round(orderSubtotal * (DiscountValue / 100m), 2, MidpointRounding.AwayFromZero)
            : DiscountValue;

        return Math.Clamp(discount, 0, orderSubtotal);
    }

    public void EnsureRedeemable(decimal orderSubtotal, DateTime nowUtc)
    {
        if (!IsActive)
            throw new DomainException("Este cupom não está ativo.");
        if (ValidFrom.HasValue && nowUtc < ValidFrom.Value)
            throw new DomainException("Este cupom ainda não é válido.");
        if (ValidUntil.HasValue && nowUtc > ValidUntil.Value)
            throw new DomainException("Este cupom expirou.");
        if (MaxUses.HasValue && UsedCount >= MaxUses.Value)
            throw new DomainException("Este cupom atingiu o limite de utilizações.");
        if (MinOrderValue.HasValue && orderSubtotal < MinOrderValue.Value)
            throw new DomainException($"O valor mínimo do pedido para este cupom é {MinOrderValue.Value:N2} Kz.");
    }

    public void RegisterUse()
    {
        UsedCount++;
        Touch();
    }

    public void Update(CouponDiscountType discountType, decimal discountValue, decimal? minOrderValue, int? maxUses, DateTime? validFrom, DateTime? validUntil)
    {
        SetDiscount(discountType, discountValue);

        if (minOrderValue is < 0)
            throw new DomainException("O valor mínimo do pedido não pode ser negativo.");
        if (maxUses is <= 0)
            throw new DomainException("O limite de utilizações deve ser maior que zero.");
        if (validFrom.HasValue && validUntil.HasValue && validFrom > validUntil)
            throw new DomainException("A data de início não pode ser depois da data de fim.");

        MinOrderValue = minOrderValue;
        MaxUses = maxUses;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    private void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("O código do cupom é obrigatório.");
        Code = code.Trim().ToUpperInvariant();
    }

    private void SetDiscount(CouponDiscountType discountType, decimal discountValue)
    {
        if (discountValue <= 0)
            throw new DomainException("O valor do desconto deve ser maior que zero.");
        if (discountType == CouponDiscountType.Percentual && discountValue > 100)
            throw new DomainException("O desconto percentual não pode ser maior que 100%.");

        DiscountType = discountType;
        DiscountValue = discountValue;
    }
}
