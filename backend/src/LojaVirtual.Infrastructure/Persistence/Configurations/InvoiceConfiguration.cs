using LojaVirtual.Domain.Invoicing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaVirtual.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.SeriesCode).IsRequired().HasMaxLength(50);
        builder.Property(i => i.SellerName).IsRequired().HasMaxLength(300);
        builder.Property(i => i.SellerNif).IsRequired().HasMaxLength(30);
        builder.Property(i => i.CustomerName).IsRequired().HasMaxLength(300);
        builder.Property(i => i.CustomerNif).HasMaxLength(30);
        builder.Property(i => i.Subtotal).HasColumnType("numeric(14,2)");
        builder.Property(i => i.VatRate).HasColumnType("numeric(5,4)");
        builder.Property(i => i.VatAmount).HasColumnType("numeric(14,2)");
        builder.Property(i => i.Total).HasColumnType("numeric(14,2)");
        builder.Ignore(i => i.FullNumber);

        // Numeração sequencial imutável: nunca pode haver dois números iguais na mesma série.
        builder.HasIndex(i => new { i.SeriesCode, i.SequenceNumber }).IsUnique();
    }
}
