using FluentValidation;
using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Catalog.Products.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Slug,
    string Brand,
    string? Description,
    Guid CategoryId,
    decimal Price) : IRequest;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}

public class UpdateProductCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Catalog.Product), request.Id);

        product.UpdateDetails(request.Name, request.Slug, request.Brand, request.Description, request.CategoryId);
        product.ChangePrice(request.Price);

        await context.SaveChangesAsync(cancellationToken);
    }
}
