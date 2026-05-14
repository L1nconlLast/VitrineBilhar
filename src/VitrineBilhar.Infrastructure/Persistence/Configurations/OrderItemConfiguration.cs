using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Quantity).IsRequired();
        builder.Property(e => e.UnitPrice).HasPrecision(12, 2);

        builder.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId);
        builder.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
