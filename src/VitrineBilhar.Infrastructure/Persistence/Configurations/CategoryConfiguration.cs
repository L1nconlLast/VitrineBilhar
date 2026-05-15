using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(120).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();

        builder.HasOne(e => e.Tenant).WithMany(e => e.Categories).HasForeignKey(e => e.TenantId);
    }
}
