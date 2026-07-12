using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Coupons.Queries;

/// <summary>
/// Pré-visualização do desconto antes do checkout (usada pela UI do carrinho). Não reserva
/// nem confirma o uso do cupom — a validação definitiva acontece de novo no CheckoutCommand.
/// </summary>
public record ValidateCouponQuery(string Code, decimal Subtotal) : IRequest<CouponValidationResultDto>;

public record CouponValidationResultDto(bool IsValid, decimal DiscountAmount, string? Message);

public class ValidateCouponQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ValidateCouponQuery, CouponValidationResultDto>
{
    public async Task<CouponValidationResultDto> Handle(ValidateCouponQuery request, CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();
        var coupon = await context.Coupons.AsNoTracking().FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken);

        if (coupon is null)
            return new CouponValidationResultDto(false, 0, "Cupom não encontrado.");

        try
        {
            coupon.EnsureRedeemable(request.Subtotal, DateTime.UtcNow);
        }
        catch (Domain.Common.DomainException ex)
        {
            return new CouponValidationResultDto(false, 0, ex.Message);
        }

        return new CouponValidationResultDto(true, coupon.CalculateDiscount(request.Subtotal), null);
    }
}
