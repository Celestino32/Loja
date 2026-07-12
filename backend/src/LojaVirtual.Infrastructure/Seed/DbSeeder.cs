using LojaVirtual.Domain.Catalog;
using LojaVirtual.Domain.Invoicing;
using LojaVirtual.Domain.Settings;
using LojaVirtual.Domain.Shipping;
using LojaVirtual.Infrastructure.Identity;
using LojaVirtual.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LojaVirtual.Infrastructure.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var configuration = services.GetRequiredService<IConfiguration>();

        await context.Database.MigrateAsync();

        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        var adminEmail = configuration["SeedAdmin:Email"] ?? "admin@lojavirtual.ao";
        var adminPassword = configuration["SeedAdmin:Password"] ?? "Trocar@123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Administrador",
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }

        if (!await context.InvoiceSeries.AnyAsync())
        {
            var currentYear = DateTime.UtcNow.Year;
            context.InvoiceSeries.Add(new InvoiceSeries($"FT {currentYear}"));
            await context.SaveChangesAsync();
        }

        if (!await context.Categories.AnyAsync())
        {
            var smartphones = new Category("Smartphones", "smartphones", "Telemóveis e acessórios");
            var informatica = new Category("Informática", "informatica", "Computadores, portáteis e periféricos");
            var eletrodomesticos = new Category("Eletrodomésticos", "eletrodomesticos", "Eletrodomésticos para casa");

            context.Categories.AddRange(smartphones, informatica, eletrodomesticos);
            await context.SaveChangesAsync();

            var phone = new Product(
                sku: "SP-SAMS-A55",
                name: "Samsung Galaxy A55 128GB",
                slug: "samsung-galaxy-a55-128gb",
                brand: "Samsung",
                price: 185000m,
                stockQuantity: 25,
                categoryId: smartphones.Id,
                description: "Smartphone Samsung Galaxy A55 com 128GB de armazenamento.");
            phone.AddAttribute("Armazenamento", "128GB");
            phone.AddAttribute("RAM", "8GB");
            phone.AddAttribute("Garantia", "12 meses");

            var laptop = new Product(
                sku: "NB-DELL-I5",
                name: "Dell Inspiron 15 Intel Core i5",
                slug: "dell-inspiron-15-intel-core-i5",
                brand: "Dell",
                price: 420000m,
                stockQuantity: 10,
                categoryId: informatica.Id,
                description: "Portátil Dell Inspiron 15 com processador Intel Core i5.");
            laptop.AddAttribute("Processador", "Intel Core i5");
            laptop.AddAttribute("RAM", "8GB");
            laptop.AddAttribute("Armazenamento", "512GB SSD");
            laptop.AddAttribute("Garantia", "12 meses");

            var fridge = new Product(
                sku: "ED-LG-FRIDGE",
                name: "Frigorífico LG 420L Duplex",
                slug: "frigorifico-lg-420l-duplex",
                brand: "LG",
                price: 350000m,
                stockQuantity: 8,
                categoryId: eletrodomesticos.Id,
                description: "Frigorífico LG Duplex 420 litros, baixo consumo de energia.");
            fridge.AddAttribute("Capacidade", "420L");
            fridge.AddAttribute("Voltagem", "220V");
            fridge.AddAttribute("Garantia", "24 meses");

            context.Products.AddRange(phone, laptop, fridge);
            await context.SaveChangesAsync();
        }

        if (!await context.ShippingZones.AnyAsync())
        {
            context.ShippingZones.AddRange(
                new ShippingZone("Luanda", "Luanda", 3500m),
                new ShippingZone("Luanda", "Talatona", 4000m),
                new ShippingZone("Luanda", "Viana", 4500m),
                new ShippingZone("Benguela", "Benguela", 9000m),
                new ShippingZone("Huambo", "Huambo", 10000m));

            await context.SaveChangesAsync();
        }

        if (!await context.StoreSettings.AnyAsync())
        {
            context.StoreSettings.Add(new StoreSettings("Celsjoscel Comércio & Serviços"));
            await context.SaveChangesAsync();
        }
    }
}
