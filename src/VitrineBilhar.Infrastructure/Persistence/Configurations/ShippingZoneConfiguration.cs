using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Infrastructure.Persistence.Configurations;

public sealed class ShippingZoneConfiguration : IEntityTypeConfiguration<ShippingZone>
{
    public void Configure(EntityTypeBuilder<ShippingZone> builder)
    {
        builder.ToTable("shipping_zones");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(120).IsRequired();
        builder.Property(e => e.StateCode).HasMaxLength(2).IsRequired();
        builder.Property(e => e.City).HasMaxLength(120);
        builder.Property(e => e.ZipCodeStart).HasMaxLength(8).IsRequired();
        builder.Property(e => e.ZipCodeEnd).HasMaxLength(8).IsRequired();
        builder.Property(e => e.FixedRate).HasPrecision(12, 2);

        builder.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId);
    }
}
