using FluentValidation;
using LojaVirtual.Application.Common;
using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Catalog.Products.Commands;

public record UploadProductImageCommand(
    Guid ProductId,
    Stream Content,
    string FileName,
    string ContentType,
    long Length) : IRequest<ProductImageDto>;

public class UploadProductImageCommandValidator : AbstractValidator<UploadProductImageCommand>
{
    public UploadProductImageCommandValidator()
    {
        RuleFor(x => x.ContentType)
            .Must(ct => ImageUploadRules.AllowedContentTypes.Contains(ct))
            .WithMessage($"Formato inválido. Aceites: {string.Join(", ", ImageUploadRules.AllowedContentTypes)}.");

        RuleFor(x => x.Length)
            .GreaterThan(0).WithMessage("Arquivo vazio.")
            .LessThanOrEqualTo(ImageUploadRules.MaxSizeBytes)
            .WithMessage($"Arquivo maior que o limite de {ImageUploadRules.MaxSizeBytes / 1024 / 1024}MB.");
    }
}

public class UploadProductImageCommandHandler(IApplicationDbContext context, IFileStorageService storage)
    : IRequestHandler<UploadProductImageCommand, ProductImageDto>
{
    public async Task<ProductImageDto> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Catalog.Product), request.ProductId);

        var url = await storage.SaveAsync(request.Content, request.FileName, "products", cancellationToken);

        var nextSortOrder = product.Images.Count == 0 ? 0 : product.Images.Max(i => i.SortOrder) + 1;
        product.AddImage(url, nextSortOrder);

        // Product já está rastreado (veio do Include acima); uma imagem nova adicionada à sua
        // coleção precisa ser marcada como Added explicitamente — como o Id é um Guid gerado no
        // cliente, o EF Core a trataria como uma entidade já existente (gerando UPDATE em vez de
        // INSERT) se dependêssemos apenas da detecção automática de mudanças.
        var savedImage = product.Images.Last();
        context.ProductImages.Add(savedImage);

        await context.SaveChangesAsync(cancellationToken);

        return new ProductImageDto(savedImage.Id, savedImage.Url, savedImage.SortOrder);
    }
}
