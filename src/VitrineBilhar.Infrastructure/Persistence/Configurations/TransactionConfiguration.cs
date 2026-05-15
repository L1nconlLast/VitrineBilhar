using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Infrastructure.Persistence.Configurations;

public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Provider).HasMaxLength(80).IsRequired();
        builder.Property(e => e.ExternalId).HasMaxLength(120);
        builder.Property(e => e.Amount).HasPrecision(12, 2);
        builder.Property(e => e.Status).HasMaxLength(40).IsRequired();

        builder.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId);
        builder.HasOne(e => e.Order).WithMany(e => e.Transactions).HasForeignKey(e => e.OrderId);
    }
}
