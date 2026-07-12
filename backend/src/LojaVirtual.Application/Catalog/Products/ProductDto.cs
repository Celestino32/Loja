namespace LojaVirtual.Application.Catalog.Products;

public record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    string Slug,
    string? Description,
    string Brand,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    Guid CategoryId,
    string? CategoryName,
    List<ProductAttributeDto> Attributes,
    List<ProductImageDto> Images);

public record ProductAttributeDto(string Key, string Value);

public record ProductImageDto(Guid Id, string Url, int SortOrder);

public record ProductListItemDto(
    Guid Id,
    string Sku,
    string Name,
    string Slug,
    string Brand,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    Guid CategoryId,
    string? CategoryName,
    string? PrimaryImageUrl);

public record PagedResult<T>(List<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
