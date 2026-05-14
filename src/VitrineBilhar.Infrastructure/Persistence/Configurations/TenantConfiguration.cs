using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(e => e.Slug)
            .HasMaxLength(80)
            .IsRequired();

        builder.HasIndex(e => e.Slug)
            .IsUnique();
    }
}
