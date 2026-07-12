using LojaVirtual.Domain.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaVirtual.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.CustomerName).IsRequired().HasMaxLength(300);
        builder.Property(r => r.Comment).HasMaxLength(2000);

        // Um pedido só pode gerar uma avaliação por produto.
        builder.HasIndex(r => new { r.OrderId, r.ProductId }).IsUnique();
        builder.HasIndex(r => r.ProductId);
    }
}
