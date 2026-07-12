using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Catalog.Categories.Queries;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;

public class GetCategoryByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.Description, c.IsActive))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Catalog.Category), request.Id);

        return category;
    }
}
