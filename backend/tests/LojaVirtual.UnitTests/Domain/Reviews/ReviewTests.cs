using LojaVirtual.Domain.Common;
using LojaVirtual.Domain.Reviews;

namespace LojaVirtual.UnitTests.Domain.Reviews;

public class ReviewTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Constructor_WithInvalidRating_ThrowsDomainException(int rating)
    {
        Assert.Throws<DomainException>(() =>
            new Review(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), rating, "Bom produto", "Cliente Teste"));
    }

    [Fact]
    public void Constructor_WithValidData_SetsProperties()
    {
        var review = new Review(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, "Excelente!", "Cliente Teste");

        Assert.Equal(5, review.Rating);
        Assert.Equal("Excelente!", review.Comment);
        Assert.Equal("Cliente Teste", review.CustomerName);
    }

    [Fact]
    public void Constructor_WithEmptyComment_StoresNull()
    {
        var review = new Review(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 4, "   ", "Cliente Teste");

        Assert.Null(review.Comment);
    }
}
