using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Coupons.Queries;

public record GetCouponsQuery : IRequest<List<CouponDto>>;

public class GetCouponsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetCouponsQuery, List<CouponDto>>
{
    public async Task<List<CouponDto>> Handle(GetCouponsQuery request, CancellationToken cancellationToken)
    {
        return await context.Coupons.AsNoTracking()
            .OrderByDescending(c => c.CreatedAtUtc)
            .Select(c => new CouponDto(c.Id, c.Code, c.DiscountType, c.DiscountValue, c.MinOrderValue, c.MaxUses, c.UsedCount, c.ValidFrom, c.ValidUntil, c.IsActive))
            .ToListAsync(cancellationToken);
    }
}
