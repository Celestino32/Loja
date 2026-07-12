using FluentValidation;
using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Coupons;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Coupons.Commands;

public record UpdateCouponCommand(
    Guid Id,
    CouponDiscountType DiscountType,
    decimal DiscountValue,
    decimal? MinOrderValue,
    int? MaxUses,
    DateTime? ValidFrom,
    DateTime? ValidUntil) : IRequest;

public class UpdateCouponCommandValidator : AbstractValidator<UpdateCouponCommand>
{
    public UpdateCouponCommandValidator()
    {
        RuleFor(x => x.DiscountValue).GreaterThan(0);
    }
}

public class UpdateCouponCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateCouponCommand>
{
    public async Task Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Coupon), request.Id);

        coupon.Update(request.DiscountType, request.DiscountValue, request.MinOrderValue, request.MaxUses, request.ValidFrom, request.ValidUntil);
        await context.SaveChangesAsync(cancellationToken);
    }
}
