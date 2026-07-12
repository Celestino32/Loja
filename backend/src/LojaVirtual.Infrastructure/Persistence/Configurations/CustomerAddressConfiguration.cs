using LojaVirtual.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaVirtual.Infrastructure.Persistence.Configurations;

public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("CustomerAddresses");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Province).IsRequired().HasMaxLength(150);
        builder.Property(a => a.Municipality).IsRequired().HasMaxLength(150);
        builder.Property(a => a.Street).IsRequired().HasMaxLength(300);
        builder.Property(a => a.Reference).HasMaxLength(300);
    }
}
