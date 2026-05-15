using VitrineBilhar.Domain.Common;

namespace VitrineBilhar.Domain.Entities;

public class ShippingZone : TenantBaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string StateCode { get; set; } = string.Empty;
    public string? City { get; set; }
    public string ZipCodeStart { get; set; } = string.Empty;
    public string ZipCodeEnd { get; set; } = string.Empty;
    public decimal FixedRate { get; set; }

    public Tenant? Tenant { get; set; }
}
