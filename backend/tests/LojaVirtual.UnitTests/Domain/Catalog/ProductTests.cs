using LojaVirtual.Domain.Catalog;
using LojaVirtual.Domain.Common;

namespace LojaVirtual.UnitTests.Domain.Catalog;

public class ProductTests
{
    private static Product CreateProduct(int stockQuantity = 10, decimal price = 100m) =>
        new(
            sku: "sku-1",
            name: "Produto Teste",
            slug: "produto-teste",
            brand: "Marca",
            price: price,
            stockQuantity: stockQuantity,
            categoryId: Guid.NewGuid());

    [Fact]
    public void Constructor_WithNegativePrice_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Product("sku-1", "Nome", "slug", "Marca", -1m, 10, Guid.NewGuid()));
    }

    [Fact]
    public void Constructor_WithNegativeStock_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Product("sku-1", "Nome", "slug", "Marca", 100m, -1, Guid.NewGuid()));
    }

    [Fact]
    public void DecreaseStock_WithSufficientStock_ReducesQuantity()
    {
        var product = CreateProduct(stockQuantity: 10);

        product.DecreaseStock(4);

        Assert.Equal(6, product.StockQuantity);
    }

    [Fact]
    public void DecreaseStock_WithInsufficientStock_ThrowsDomainException()
    {
        var product = CreateProduct(stockQuantity: 3);

        Assert.Throws<DomainException>(() => product.DecreaseStock(4));
    }

    [Fact]
    public void DecreaseStock_WithZeroOrNegativeQuantity_ThrowsDomainException()
    {
        var product = CreateProduct(stockQuantity: 10);

        Assert.Throws<DomainException>(() => product.DecreaseStock(0));
    }

    [Fact]
    public void IncreaseStock_AddsToQuantity()
    {
        var product = CreateProduct(stockQuantity: 5);

        product.IncreaseStock(10);

        Assert.Equal(15, product.StockQuantity);
    }

    [Fact]
    public void ChangePrice_WithNegativeValue_ThrowsDomainException()
    {
        var product = CreateProduct();

        Assert.Throws<DomainException>(() => product.ChangePrice(-10m));
    }

    [Fact]
    public void AddAttribute_WithSameKeyTwice_ReplacesPreviousValue()
    {
        var product = CreateProduct();

        product.AddAttribute("Voltagem", "110V");
        product.AddAttribute("Voltagem", "220V");

        var attribute = Assert.Single(product.Attributes);
        Assert.Equal("220V", attribute.Value);
    }
}
