using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Catalog;

public class Product : AuditableEntity
{
    public string Sku { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? Description { get; private set; }
    public string Brand { get; private set; } = default!;

    /// <summary>Preço unitário em Kwanza (AOA), sem IVA.</summary>
    public decimal Price { get; private set; }

    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private readonly List<ProductAttribute> _attributes = [];
    public IReadOnlyCollection<ProductAttribute> Attributes => _attributes.AsReadOnly();

    private readonly List<ProductImage> _images = [];
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    private Product()
    {
    }

    public Product(
        string sku,
        string name,
        string slug,
        string brand,
        decimal price,
        int stockQuantity,
        Guid categoryId,
        string? description = null)
    {
        SetSku(sku);
        SetName(name);
        SetSlug(slug);
        SetBrand(brand);
        ChangePrice(price);
        SetInitialStock(stockQuantity);
        Description = description;
        CategoryId = categoryId;
    }

    public void UpdateDetails(string name, string slug, string brand, string? description, Guid categoryId)
    {
        SetName(name);
        SetSlug(slug);
        SetBrand(brand);
        Description = description;
        CategoryId = categoryId;
        Touch();
    }

    public void ChangePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new DomainException("O preço não pode ser negativo.");
        Price = newPrice;
        Touch();
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("A quantidade a repor deve ser maior que zero.");
        StockQuantity += quantity;
        Touch();
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("A quantidade a retirar deve ser maior que zero.");
        if (quantity > StockQuantity)
            throw new DomainException($"Estoque insuficiente para o produto '{Name}'. Disponível: {StockQuantity}.");
        StockQuantity -= quantity;
        Touch();
    }

    public void AddAttribute(string key, string value)
    {
        _attributes.RemoveAll(a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        _attributes.Add(new ProductAttribute(Id, key, value));
        Touch();
    }

    public void AddImage(string url, int sortOrder = 0)
    {
        _images.Add(new ProductImage(Id, url, sortOrder));
        Touch();
    }

    /// <summary>Remove a imagem do produto. Não apaga o arquivo em si — isso é responsabilidade
    /// da camada de aplicação, que decide se o storage deve ser limpo.</summary>
    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new DomainException("Imagem não encontrada neste produto.");
        _images.Remove(image);
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

    private void SetSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("O SKU do produto é obrigatório.");
        Sku = sku.Trim().ToUpperInvariant();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do produto é obrigatório.");
        Name = name.Trim();
    }

    private void SetSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("O slug do produto é obrigatório.");
        Slug = slug.Trim().ToLowerInvariant();
    }

    private void SetBrand(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new DomainException("A marca do produto é obrigatória.");
        Brand = brand.Trim();
    }

    private void SetInitialStock(int stockQuantity)
    {
        if (stockQuantity < 0)
            throw new DomainException("A quantidade em estoque não pode ser negativa.");
        StockQuantity = stockQuantity;
    }
}
