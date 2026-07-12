using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Catalog.Products.Commands;

/// <summary>Ajusta o estoque: quantidade positiva repõe, negativa retira.</summary>
public record AdjustStockCommand(Guid ProductId, int Quantity) : IRequest;

public class AdjustStockCommandHandler(IApplicationDbContext context) : IRequestHandler<AdjustStockCommand>
{
    public async Task Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Catalog.Product), request.ProductId);

        if (request.Quantity > 0)
            product.IncreaseStock(request.Quantity);
        else if (request.Quantity < 0)
            product.DecreaseStock(-request.Quantity);

        await context.SaveChangesAsync(cancellationToken);
    }
}
