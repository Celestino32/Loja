using LojaVirtual.Domain.Coupons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaVirtual.Infrastructure.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
        builder.Property(c => c.DiscountType).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.DiscountValue).HasColumnType("numeric(14,2)");
        builder.Property(c => c.MinOrderValue).HasColumnType("numeric(14,2)");

        builder.HasIndex(c => c.Code).IsUnique();
    }
}
