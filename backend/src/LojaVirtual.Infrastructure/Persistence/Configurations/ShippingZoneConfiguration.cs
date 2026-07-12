using LojaVirtual.Domain.Shipping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaVirtual.Infrastructure.Persistence.Configurations;

public class ShippingZoneConfiguration : IEntityTypeConfiguration<ShippingZone>
{
    public void Configure(EntityTypeBuilder<ShippingZone> builder)
    {
        builder.ToTable("ShippingZones");
        builder.HasKey(z => z.Id);

        builder.Property(z => z.Province).IsRequired().HasMaxLength(150);
        builder.Property(z => z.Municipality).IsRequired().HasMaxLength(150);
        builder.Property(z => z.Cost).HasColumnType("numeric(14,2)");

        builder.HasIndex(z => new { z.Province, z.Municipality }).IsUnique();
    }
}
