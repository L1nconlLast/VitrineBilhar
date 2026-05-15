using VitrineBilhar.Domain.Common;

namespace VitrineBilhar.Domain.Entities;

public class Order : TenantBaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerDocument { get; set; }
    public string ShippingZipCode { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "pending";

    public Tenant? Tenant { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
