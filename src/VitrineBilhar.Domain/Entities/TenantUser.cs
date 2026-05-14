using VitrineBilhar.Domain.Common;

namespace VitrineBilhar.Domain.Entities;

public class TenantUser : TenantBaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "admin";

    public Tenant? Tenant { get; set; }
}
