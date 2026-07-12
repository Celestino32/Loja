using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Catalog.Products.Queries;

public record GetProductBySlugQuery(string Slug) : IRequest<ProductDto>;

public class GetProductBySlugQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductBySlugQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
    {
        var product = await context.Products.AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Attributes)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Catalog.Product), request.Slug);

        return new ProductDto(
            product.Id,
            product.Sku,
            product.Name,
            product.Slug,
            product.Description,
            product.Brand,
            product.Price,
            product.StockQuantity,
            product.IsActive,
            product.CategoryId,
            product.Category?.Name,
            product.Attributes.Select(a => new ProductAttributeDto(a.Key, a.Value)).ToList(),
            product.Images.OrderBy(i => i.SortOrder).Select(i => new ProductImageDto(i.Id, i.Url, i.SortOrder)).ToList());
    }
}
