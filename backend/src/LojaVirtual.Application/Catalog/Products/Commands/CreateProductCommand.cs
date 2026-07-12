using FluentValidation;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Catalog;
using MediatR;

namespace LojaVirtual.Application.Catalog.Products.Commands;

public record CreateProductCommand(
    string Sku,
    string Name,
    string Slug,
    string Brand,
    decimal Price,
    int StockQuantity,
    Guid CategoryId,
    string? Description,
    List<ProductAttributeDto>? Attributes,
    List<string>? ImageUrls) : IRequest<Guid>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public class CreateProductCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            request.Sku,
            request.Name,
            request.Slug,
            request.Brand,
            request.Price,
            request.StockQuantity,
            request.CategoryId,
            request.Description);

        foreach (var attribute in request.Attributes ?? [])
            product.AddAttribute(attribute.Key, attribute.Value);

        var sortOrder = 0;
        foreach (var url in request.ImageUrls ?? [])
            product.AddImage(url, sortOrder++);

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
