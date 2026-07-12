using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Reports.Queries;

public record GetLowStockProductsQuery(int Threshold = 5) : IRequest<List<LowStockProductDto>>;

public class GetLowStockProductsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetLowStockProductsQuery, List<LowStockProductDto>>
{
    public async Task<List<LowStockProductDto>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        return await context.Products.AsNoTracking()
            .Where(p => p.IsActive && p.StockQuantity <= request.Threshold)
            .OrderBy(p => p.StockQuantity)
            .Select(p => new LowStockProductDto(p.Id, p.Name, p.Sku, p.StockQuantity))
            .ToListAsync(cancellationToken);
    }
}
