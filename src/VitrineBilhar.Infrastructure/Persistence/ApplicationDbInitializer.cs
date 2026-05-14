using Microsoft.EntityFrameworkCore;
using VitrineBilhar.Application.Abstractions;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Infrastructure.Persistence;

public sealed class ApplicationDbInitializer(ApplicationDbContext dbContext, ITenantContext tenantContext)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var hasMigrations = dbContext.Database.GetMigrations().Any();
        if (hasMigrations)
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }

        var hasTenant = await dbContext.Tenants.IgnoreQueryFilters().AnyAsync(cancellationToken);
        if (hasTenant)
        {
            return;
        }

        var tenant = new Tenant
        {
            Name = "Tenant Demo",
            Slug = "demo"
        };

        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync(cancellationToken);

        tenantContext.SetTenant(tenant.Id, tenant.Slug);

        dbContext.Products.Add(new Product
        {
            Name = "Taco Profissional",
            Description = "Produto de demonstração",
            Price = 299.90m,
            IsActive = true
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        tenantContext.Clear();
    }
}
