namespace VitrineBilhar.Application.Abstractions;

public interface ITenantContext
{
    Guid? TenantId { get; }
    string? TenantSlug { get; }
    bool IsResolved { get; }

    void SetTenant(Guid tenantId, string? tenantSlug = null);
    void Clear();
}
