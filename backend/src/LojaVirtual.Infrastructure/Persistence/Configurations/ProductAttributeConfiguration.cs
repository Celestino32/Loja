using LojaVirtual.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaVirtual.Infrastructure.Persistence.Configurations;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Key).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Value).IsRequired().HasMaxLength(500);

        builder.HasIndex(a => new { a.ProductId, a.Key }).IsUnique();
    }
}
