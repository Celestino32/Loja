using LojaVirtual.Domain.Ordering;
using LojaVirtual.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaVirtual.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(o => o.FulfillmentType).HasConversion<string>().HasMaxLength(50);
        builder.Property(o => o.ShippingCost).HasColumnType("numeric(14,2)");
        builder.Property(o => o.DiscountTotal).HasColumnType("numeric(14,2)");
        builder.Property(o => o.Total).HasColumnType("numeric(14,2)");
        builder.Property(o => o.ShippingProvince).HasMaxLength(150);
        builder.Property(o => o.ShippingMunicipality).HasMaxLength(150);
        builder.Property(o => o.ShippingStreet).HasMaxLength(300);
        builder.Property(o => o.ShippingReference).HasMaxLength(300);
        builder.Property(o => o.CouponCode).HasMaxLength(50);

        builder.HasIndex(o => o.OrderNumber).IsUnique();

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(o => o.Payment)
            .WithOne()
            .HasForeignKey<Payment>(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
