using Microsoft.Extensions.DependencyInjection;
using VitrineBilhar.Application.Products;

namespace VitrineBilhar.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        return services;
    }
}
