namespace LojaVirtual.Application.Catalog.Categories;

public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    bool IsActive);
