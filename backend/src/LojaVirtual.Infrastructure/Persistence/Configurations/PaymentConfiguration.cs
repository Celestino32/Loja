using LojaVirtual.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaVirtual.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.Amount).HasColumnType("numeric(14,2)");
        builder.Property(p => p.ExternalReference).HasMaxLength(200);
        builder.Property(p => p.Instructions).HasMaxLength(1000);

        builder.HasIndex(p => p.OrderId).IsUnique();
    }
}
