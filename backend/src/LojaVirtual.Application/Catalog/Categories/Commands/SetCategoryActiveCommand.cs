using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Catalog.Categories.Commands;

public record SetCategoryActiveCommand(Guid Id, bool IsActive) : IRequest;

public class SetCategoryActiveCommandHandler(IApplicationDbContext context) : IRequestHandler<SetCategoryActiveCommand>
{
    public async Task Handle(SetCategoryActiveCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Catalog.Category), request.Id);

        if (request.IsActive)
            category.Activate();
        else
            category.Deactivate();

        await context.SaveChangesAsync(cancellationToken);
    }
}
