using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Ordering;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Reports.Queries;

public record GetTopProductsQuery(DateOnly From, DateOnly To, int Limit = 10) : IRequest<List<TopProductDto>>;

public class GetTopProductsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetTopProductsQuery, List<TopProductDto>>
{
    public async Task<List<TopProductDto>> Handle(GetTopProductsQuery request, CancellationToken cancellationToken)
    {
        var fromUtc = request.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toUtc = request.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var items = await (
            from o in context.Orders.AsNoTracking()
            where o.Status != OrderStatus.Pendente && o.Status != OrderStatus.Cancelado
            where o.CreatedAtUtc >= fromUtc && o.CreatedAtUtc <= toUtc
            from i in o.Items
            select new { i.ProductId, i.ProductName, i.Quantity, i.UnitPrice }
        ).ToListAsync(cancellationToken);

        return items
            .GroupBy(i => new { i.ProductId, i.ProductName })
            .Select(g => new TopProductDto(g.Key.ProductId, g.Key.ProductName, g.Sum(i => i.Quantity), g.Sum(i => i.Quantity * i.UnitPrice)))
            .OrderByDescending(p => p.QuantitySold)
            .Take(request.Limit)
            .ToList();
    }
}
