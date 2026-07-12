using System.Text;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Application.Invoicing;
using LojaVirtual.Infrastructure.Auth;
using LojaVirtual.Infrastructure.Identity;
using LojaVirtual.Infrastructure.Invoicing;
using LojaVirtual.Infrastructure.Payments;
using LojaVirtual.Infrastructure.Persistence;
using LojaVirtual.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;

namespace LojaVirtual.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("A connection string 'DefaultConnection' não foi configurada.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<JwtTokenGenerator>();

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("A seção de configuração 'Jwt' não foi configurada.");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();

        services.Configure<BankTransferSettings>(configuration.GetSection(BankTransferSettings.SectionName));
        services.Configure<AppyPaySettings>(configuration.GetSection(AppyPaySettings.SectionName));

        services.AddScoped<IPaymentGateway, BankTransferPaymentGateway>();
        services.AddScoped<IPaymentGateway, ReferencePaymentGateway>();

        var appyPaySettings = configuration.GetSection(AppyPaySettings.SectionName).Get<AppyPaySettings>() ?? new AppyPaySettings();
        services.AddHttpClient<AppyPayGateway>(client =>
        {
            client.BaseAddress = new Uri(appyPaySettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        });
        services.AddScoped<IPaymentGateway>(provider => provider.GetRequiredService<AppyPayGateway>());

        QuestPDF.Settings.License = LicenseType.Community;
        services.AddScoped<IInvoicePdfGenerator, QuestPdfInvoiceGenerator>();
        services.AddScoped<ISaftExporter, XmlSaftExporter>();

        services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }
}
