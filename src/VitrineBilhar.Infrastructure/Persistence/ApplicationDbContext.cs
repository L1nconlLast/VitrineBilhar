using Microsoft.EntityFrameworkCore;
using VitrineBilhar.Application.Abstractions;
using VitrineBilhar.Domain.Common;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantContext tenantContext)
    : DbContext(options), IApplicationDbContext
{
    private Guid? CurrentTenantId => tenantContext.TenantId;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantUser> TenantUsers => Set<TenantUser>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ShippingZone> ShippingZones => Set<ShippingZone>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<TenantUser>().HasQueryFilter(e => CurrentTenantId.HasValue && e.TenantId == CurrentTenantId.Value);
        modelBuilder.Entity<Category>().HasQueryFilter(e => CurrentTenantId.HasValue && e.TenantId == CurrentTenantId.Value);
        modelBuilder.Entity<Product>().HasQueryFilter(e => CurrentTenantId.HasValue && e.TenantId == CurrentTenantId.Value);
        modelBuilder.Entity<ShippingZone>().HasQueryFilter(e => CurrentTenantId.HasValue && e.TenantId == CurrentTenantId.Value);
        modelBuilder.Entity<Order>().HasQueryFilter(e => CurrentTenantId.HasValue && e.TenantId == CurrentTenantId.Value);
        modelBuilder.Entity<OrderItem>().HasQueryFilter(e => CurrentTenantId.HasValue && e.TenantId == CurrentTenantId.Value);
        modelBuilder.Entity<Transaction>().HasQueryFilter(e => CurrentTenantId.HasValue && e.TenantId == CurrentTenantId.Value);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantEntries = ChangeTracker.Entries<ITenantEntity>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (tenantEntries.Count != 0)
        {
            if (!CurrentTenantId.HasValue)
            {
                throw new InvalidOperationException("Tenant não resolvido para operação de escrita.");
            }

            var currentTenantId = CurrentTenantId.Value;

            foreach (var entry in tenantEntries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.TenantId == Guid.Empty)
                    {
                        entry.Entity.TenantId = currentTenantId;
                    }
                    else if (entry.Entity.TenantId != currentTenantId)
                    {
                        throw new InvalidOperationException("Não é permitido gravar dados em outro tenant.");
                    }
                }
                else if (entry.Entity.TenantId != currentTenantId)
                {
                    throw new InvalidOperationException("Não é permitido alterar dados de outro tenant.");
                }
            }
        }

        foreach (var trackedEntity in ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Modified))
        {
            trackedEntity.Entity.UpdatedAtUtc = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
