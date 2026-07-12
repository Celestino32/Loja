using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Catalog.Products.Commands;

public record SetProductActiveCommand(Guid Id, bool IsActive) : IRequest;

public class SetProductActiveCommandHandler(IApplicationDbContext context) : IRequestHandler<SetProductActiveCommand>
{
    public async Task Handle(SetProductActiveCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Catalog.Product), request.Id);

        if (request.IsActive)
            product.Activate();
        else
            product.Deactivate();

        await context.SaveChangesAsync(cancellationToken);
    }
}
