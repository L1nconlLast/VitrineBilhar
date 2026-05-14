using VitrineBilhar.Domain.Common;

namespace VitrineBilhar.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<TenantUser> Users { get; set; } = new List<TenantUser>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
