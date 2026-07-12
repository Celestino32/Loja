using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Catalog;
using LojaVirtual.Domain.Coupons;
using LojaVirtual.Domain.Identity;
using LojaVirtual.Domain.Invoicing;
using LojaVirtual.Domain.Ordering;
using LojaVirtual.Domain.Payments;
using LojaVirtual.Domain.Reviews;
using LojaVirtual.Domain.Settings;
using LojaVirtual.Domain.Shipping;
using LojaVirtual.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options), IApplicationDbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceSeries> InvoiceSeries => Set<InvoiceSeries>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ShippingZone> ShippingZones => Set<ShippingZone>();
    public DbSet<StoreSettings> StoreSettings => Set<StoreSettings>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Coupon> Coupons => Set<Coupon>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
