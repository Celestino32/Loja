using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Catalog;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string Url { get; private set; } = default!;
    public int SortOrder { get; private set; }

    private ProductImage()
    {
    }

    public ProductImage(Guid productId, string url, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("A URL da imagem é obrigatória.");

        ProductId = productId;
        Url = url.Trim();
        SortOrder = sortOrder;
    }
}
