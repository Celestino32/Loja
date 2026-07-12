using LojaVirtual.Domain.Invoicing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaVirtual.Infrastructure.Persistence.Configurations;

public class InvoiceSeriesConfiguration : IEntityTypeConfiguration<InvoiceSeries>
{
    public void Configure(EntityTypeBuilder<InvoiceSeries> builder)
    {
        builder.ToTable("InvoiceSeries");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(s => s.Code).IsUnique();
    }
}
