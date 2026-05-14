using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CustomerName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.CustomerDocument).HasMaxLength(30);
        builder.Property(e => e.ShippingZipCode).HasMaxLength(8).IsRequired();
        builder.Property(e => e.Status).HasMaxLength(40).IsRequired();
        builder.Property(e => e.TotalAmount).HasPrecision(12, 2);

        builder.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId);
        builder.HasMany(e => e.Items).WithOne(e => e.Order).HasForeignKey(e => e.OrderId);
    }
}
