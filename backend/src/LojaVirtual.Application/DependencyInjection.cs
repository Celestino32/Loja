using FluentValidation;
using LojaVirtual.Application.Common.Behaviors;
using LojaVirtual.Application.Common.Settings;
using LojaVirtual.Application.Invoicing;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LojaVirtual.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.Configure<CompanySettings>(configuration.GetSection(CompanySettings.SectionName));
        services.AddScoped<IInvoiceIssuer, InvoiceIssuer>();

        return services;
    }
}
