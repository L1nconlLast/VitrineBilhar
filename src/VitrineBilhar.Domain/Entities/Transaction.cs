using VitrineBilhar.Domain.Common;

namespace VitrineBilhar.Domain.Entities;

public class Transaction : TenantBaseEntity
{
    public Guid OrderId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "pending";

    public Tenant? Tenant { get; set; }
    public Order? Order { get; set; }
}
