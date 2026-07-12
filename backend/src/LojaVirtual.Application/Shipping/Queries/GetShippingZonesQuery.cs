using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Shipping.Queries;

public record GetShippingZonesQuery(bool OnlyActive = false) : IRequest<List<ShippingZoneDto>>;

public class GetShippingZonesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetShippingZonesQuery, List<ShippingZoneDto>>
{
    public async Task<List<ShippingZoneDto>> Handle(GetShippingZonesQuery request, CancellationToken cancellationToken)
    {
        var query = context.ShippingZones.AsNoTracking().AsQueryable();

        if (request.OnlyActive)
            query = query.Where(z => z.IsActive);

        return await query
            .OrderBy(z => z.Province).ThenBy(z => z.Municipality)
            .Select(z => new ShippingZoneDto(z.Id, z.Province, z.Municipality, z.Cost, z.IsActive))
            .ToListAsync(cancellationToken);
    }
}
