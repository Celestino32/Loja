using LojaVirtual.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaVirtual.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Sku).IsRequired().HasMaxLength(64);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(300);
        builder.Property(p => p.Slug).IsRequired().HasMaxLength(300);
        builder.Property(p => p.Brand).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Description).HasMaxLength(4000);
        builder.Property(p => p.Price).HasColumnType("numeric(14,2)");

        builder.HasIndex(p => p.Sku).IsUnique();
        builder.HasIndex(p => p.Slug).IsUnique();

        builder.HasMany(p => p.Attributes)
            .WithOne()
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Attributes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(p => p.Images).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
