using VitrineBilhar.Application.Abstractions;

namespace VitrineBilhar.Infrastructure.MultiTenancy;

public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public string? TenantSlug { get; private set; }
    public bool IsResolved => TenantId.HasValue;

    public void SetTenant(Guid tenantId, string? tenantSlug = null)
    {
        TenantId = tenantId;
        TenantSlug = tenantSlug;
    }

    public void Clear()
    {
        TenantId = null;
        TenantSlug = null;
    }
}
