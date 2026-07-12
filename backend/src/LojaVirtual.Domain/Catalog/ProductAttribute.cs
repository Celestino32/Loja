using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Catalog;

/// <summary>
/// Especificação técnica dinâmica de um produto eletrônico (ex: Voltagem=220V, Garantia=12 meses).
/// </summary>
public class ProductAttribute : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string Key { get; private set; } = default!;
    public string Value { get; private set; } = default!;

    private ProductAttribute()
    {
    }

    public ProductAttribute(Guid productId, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("A chave do atributo é obrigatória.");
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("O valor do atributo é obrigatório.");

        ProductId = productId;
        Key = key.Trim();
        Value = value.Trim();
    }
}
