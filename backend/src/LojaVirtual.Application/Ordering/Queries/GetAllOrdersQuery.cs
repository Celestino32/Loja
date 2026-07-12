using LojaVirtual.Application.Catalog.Products;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Ordering;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Ordering.Queries;

public record GetAllOrdersQuery(OrderStatus? Status = null, string? Search = null, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<OrderSummaryDto>>;

public class GetAllOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllOrdersQuery, PagedResult<OrderSummaryDto>>
{
    public async Task<PagedResult<OrderSummaryDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = context.Orders.AsNoTracking().AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(o => o.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(o => o.OrderNumber.ToLower().Contains(request.Search.Trim().ToLowerInvariant()));

        var totalCount = await query.CountAsync(cancellationToken);

        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .OrderByDescending(o => o.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderSummaryDto(o.Id, o.OrderNumber, o.Status, o.Total, o.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<OrderSummaryDto>(items, page, pageSize, totalCount);
    }
}
