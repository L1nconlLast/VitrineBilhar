using Microsoft.EntityFrameworkCore;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<TenantUser> TenantUsers { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<ShippingZone> ShippingZones { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
