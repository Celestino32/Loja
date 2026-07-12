using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Catalog.Products.Commands;

public record RemoveProductImageCommand(Guid ProductId, Guid ImageId) : IRequest;

public class RemoveProductImageCommandHandler(IApplicationDbContext context, IFileStorageService storage)
    : IRequestHandler<RemoveProductImageCommand>
{
    public async Task Handle(RemoveProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Catalog.Product), request.ProductId);

        var image = product.Images.FirstOrDefault(i => i.Id == request.ImageId)
            ?? throw new NotFoundException(nameof(Domain.Catalog.ProductImage), request.ImageId);
        var url = image.Url;

        product.RemoveImage(request.ImageId);
        await context.SaveChangesAsync(cancellationToken);

        storage.Delete(url);
    }
}
