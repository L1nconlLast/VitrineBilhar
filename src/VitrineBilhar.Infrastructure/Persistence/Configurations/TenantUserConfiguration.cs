using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Infrastructure.Persistence.Configurations;

public sealed class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
{
    public void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        builder.ToTable("tenant_users");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Email).HasMaxLength(200).IsRequired();
        builder.Property(e => e.UserId).HasMaxLength(120).IsRequired();
        builder.Property(e => e.Role).HasMaxLength(40).IsRequired();

        builder.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
        builder.HasOne(e => e.Tenant).WithMany(e => e.Users).HasForeignKey(e => e.TenantId);
    }
}
