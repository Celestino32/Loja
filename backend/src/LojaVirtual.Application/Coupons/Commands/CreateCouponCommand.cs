using FluentValidation;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Coupons;
using MediatR;

namespace LojaVirtual.Application.Coupons.Commands;

public record CreateCouponCommand(
    string Code,
    CouponDiscountType DiscountType,
    decimal DiscountValue,
    decimal? MinOrderValue,
    int? MaxUses,
    DateTime? ValidFrom,
    DateTime? ValidUntil) : IRequest<Guid>;

public class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DiscountValue).GreaterThan(0);
    }
}

public class CreateCouponCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateCouponCommand, Guid>
{
    public async Task<Guid> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = new Coupon(
            request.Code,
            request.DiscountType,
            request.DiscountValue,
            request.MinOrderValue,
            request.MaxUses,
            request.ValidFrom,
            request.ValidUntil);

        context.Coupons.Add(coupon);
        await context.SaveChangesAsync(cancellationToken);
        return coupon.Id;
    }
}
