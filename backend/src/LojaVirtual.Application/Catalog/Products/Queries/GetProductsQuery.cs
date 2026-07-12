using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Catalog.Products.Queries;

public record GetProductsQuery(
    string? Search = null,
    Guid? CategoryId = null,
    bool OnlyActive = true,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<ProductListItemDto>>;

public class GetProductsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductListItemDto>>
{
    public async Task<PagedResult<ProductListItemDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Products.AsNoTracking().AsQueryable();

        if (request.OnlyActive)
            query = query.Where(p => p.IsActive);

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLowerInvariant();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.Brand.ToLower().Contains(term) ||
                p.Sku.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListItemDto(
                p.Id,
                p.Sku,
                p.Name,
                p.Slug,
                p.Brand,
                p.Price,
                p.StockQuantity,
                p.IsActive,
                p.CategoryId,
                p.Category != null ? p.Category.Name : null,
                p.Images.OrderBy(i => i.SortOrder).Select(i => i.Url).FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductListItemDto>(items, page, pageSize, totalCount);
    }
}
