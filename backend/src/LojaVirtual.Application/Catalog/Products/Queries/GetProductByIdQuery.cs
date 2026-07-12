using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Catalog.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public class GetProductByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await context.Products.AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Attributes)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Catalog.Product), request.Id);

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
