using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.Price).HasPrecision(12, 2);

        builder.HasIndex(e => new { e.TenantId, e.Name });

        builder.HasOne(e => e.Tenant).WithMany(e => e.Products).HasForeignKey(e => e.TenantId);
        builder.HasOne(e => e.Category).WithMany(e => e.Products).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.SetNull);
    }
}
