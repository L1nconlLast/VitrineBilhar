using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VitrineBilhar.Application.Abstractions;
using VitrineBilhar.Domain.Shipping;
using VitrineBilhar.Infrastructure.MultiTenancy;
using VitrineBilhar.Infrastructure.Options;
using VitrineBilhar.Infrastructure.Persistence;
using VitrineBilhar.Infrastructure.Services;

namespace VitrineBilhar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MultiTenantOptions>(configuration.GetSection(MultiTenantOptions.SectionName));
        services.Configure<MelhorEnvioOptions>(configuration.GetSection(MelhorEnvioOptions.SectionName));

        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantResolver, TenantResolver>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não foi configurada.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddHttpClient("ViaCep", client => client.BaseAddress = new Uri("https://viacep.com.br/"));

        services.AddHttpClient("MelhorEnvio", (provider, client) =>
        {
            var settings = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MelhorEnvioOptions>>().Value;
            client.BaseAddress = new Uri(settings.BaseUrl);
        });

        services.AddScoped<FixedRateShippingService>();
        services.AddScoped<MelhorEnvioShippingService>();
        services.AddScoped<IShippingService, HybridShippingService>();
        services.AddScoped<ApplicationDbInitializer>();

        return services;
    }
}
