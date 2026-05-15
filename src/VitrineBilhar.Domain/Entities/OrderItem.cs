using VitrineBilhar.Domain.Common;

namespace VitrineBilhar.Domain.Entities;

public class OrderItem : TenantBaseEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Tenant? Tenant { get; set; }
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}
