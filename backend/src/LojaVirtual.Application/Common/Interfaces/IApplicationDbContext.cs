using LojaVirtual.Domain.Catalog;
using LojaVirtual.Domain.Coupons;
using LojaVirtual.Domain.Identity;
using LojaVirtual.Domain.Invoicing;
using LojaVirtual.Domain.Ordering;
using LojaVirtual.Domain.Payments;
using LojaVirtual.Domain.Reviews;
using LojaVirtual.Domain.Settings;
using LojaVirtual.Domain.Shipping;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<Customer> Customers { get; }
    DbSet<CustomerAddress> CustomerAddresses { get; }
    DbSet<Order> Orders { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceSeries> InvoiceSeries { get; }
    DbSet<Payment> Payments { get; }
    DbSet<ShippingZone> ShippingZones { get; }
    DbSet<StoreSettings> StoreSettings { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Coupon> Coupons { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
