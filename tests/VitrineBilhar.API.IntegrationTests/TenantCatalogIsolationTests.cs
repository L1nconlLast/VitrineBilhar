using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using VitrineBilhar.Application.Abstractions;
using VitrineBilhar.Application.Products;
using VitrineBilhar.Domain.Entities;
using VitrineBilhar.Infrastructure.Persistence;

namespace VitrineBilhar.API.IntegrationTests;

public sealed class TenantCatalogIsolationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TenantCatalogIsolationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Catalog_BySlug_ShouldReturnOnlyProductsFromTenant()
    {
        var slugA = $"tenant-a-{Guid.NewGuid():N}";
        var slugB = $"tenant-b-{Guid.NewGuid():N}";

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();

            var tenantA = new Tenant { Name = "Tenant A", Slug = slugA };
            var tenantB = new Tenant { Name = "Tenant B", Slug = slugB };

            dbContext.Tenants.AddRange(tenantA, tenantB);
            await dbContext.SaveChangesAsync();

            tenantContext.SetTenant(tenantA.Id, tenantA.Slug);
            dbContext.Products.Add(new Product
            {
                Name = "Produto A",
                Description = "Produto do tenant A",
                Price = 100m,
                IsActive = true
            });
            await dbContext.SaveChangesAsync();

            tenantContext.SetTenant(tenantB.Id, tenantB.Slug);
            dbContext.Products.Add(new Product
            {
                Name = "Produto B",
                Description = "Produto do tenant B",
                Price = 200m,
                IsActive = true
            });
            await dbContext.SaveChangesAsync();

            tenantContext.Clear();
        }

        var client = _factory.CreateClient();
        var products = await client.GetFromJsonAsync<List<ProductDto>>($"/api/catalog/{slugA}/products");

        Assert.NotNull(products);
        Assert.Single(products!);
        Assert.Equal("Produto A", products[0].Name);
    }
}
