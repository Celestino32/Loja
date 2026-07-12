using LojaVirtual.Domain.Catalog;
using LojaVirtual.Domain.Common;

namespace LojaVirtual.UnitTests.Domain.Catalog;

public class CategoryTests
{
    [Fact]
    public void Constructor_WithEmptyName_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new Category("", "slug"));
    }

    [Fact]
    public void Constructor_NormalizesSlugToLowerCase()
    {
        var category = new Category("Smartphones", "SmartPhones");

        Assert.Equal("smartphones", category.Slug);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var category = new Category("Informática", "informatica");

        category.Deactivate();

        Assert.False(category.IsActive);
    }
}
