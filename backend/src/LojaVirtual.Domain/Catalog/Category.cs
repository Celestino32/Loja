using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Catalog;

public class Category : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Product> _products = [];
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category()
    {
    }

    public Category(string name, string slug, string? description = null)
    {
        SetName(name);
        SetSlug(slug);
        Description = description;
    }

    public void Update(string name, string slug, string? description)
    {
        SetName(name);
        SetSlug(slug);
        Description = description;
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome da categoria é obrigatório.");
        Name = name.Trim();
    }

    private void SetSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("O slug da categoria é obrigatório.");
        Slug = slug.Trim().ToLowerInvariant();
    }
}
