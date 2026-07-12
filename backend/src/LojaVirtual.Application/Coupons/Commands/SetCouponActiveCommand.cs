using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Coupons;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Coupons.Commands;

public record SetCouponActiveCommand(Guid Id, bool IsActive) : IRequest;

public class SetCouponActiveCommandHandler(IApplicationDbContext context) : IRequestHandler<SetCouponActiveCommand>
{
    public async Task Handle(SetCouponActiveCommand request, CancellationToken cancellationToken)
    {
        var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Coupon), request.Id);

        if (request.IsActive)
            coupon.Activate();
        else
            coupon.Deactivate();

        await context.SaveChangesAsync(cancellationToken);
    }
}
