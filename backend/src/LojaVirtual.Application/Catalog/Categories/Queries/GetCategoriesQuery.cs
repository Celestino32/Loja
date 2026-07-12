using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Catalog.Categories.Queries;

public record GetCategoriesQuery(bool OnlyActive = false) : IRequest<List<CategoryDto>>;

public class GetCategoriesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = context.Categories.AsNoTracking().AsQueryable();

        if (request.OnlyActive)
            query = query.Where(c => c.IsActive);

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.Description, c.IsActive))
            .ToListAsync(cancellationToken);
    }
}
