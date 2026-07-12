using FluentValidation;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Catalog;
using MediatR;

namespace LojaVirtual.Application.Catalog.Categories.Commands;

public record CreateCategoryCommand(string Name, string Slug, string? Description) : IRequest<Guid>;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200);
    }
}

public class CreateCategoryCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category(request.Name, request.Slug, request.Description);
        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
