using VitrineBilhar.Domain.Common;

namespace VitrineBilhar.Domain.Entities;

public class Category : TenantBaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Tenant? Tenant { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
