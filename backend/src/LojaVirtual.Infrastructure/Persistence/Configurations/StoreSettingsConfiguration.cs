using LojaVirtual.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaVirtual.Infrastructure.Persistence.Configurations;

public class StoreSettingsConfiguration : IEntityTypeConfiguration<StoreSettings>
{
    public void Configure(EntityTypeBuilder<StoreSettings> builder)
    {
        builder.ToTable("StoreSettings");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StoreName).IsRequired().HasMaxLength(200);
        builder.Property(s => s.LogoUrl).HasMaxLength(500);
    }
}
