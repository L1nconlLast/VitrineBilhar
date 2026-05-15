using VitrineBilhar.Domain.Common;

namespace VitrineBilhar.Domain.Entities;

public class Product : TenantBaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? CategoryId { get; set; }

    public Tenant? Tenant { get; set; }
    public Category? Category { get; set; }
}
