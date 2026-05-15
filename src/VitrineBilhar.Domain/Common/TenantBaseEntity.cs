namespace VitrineBilhar.Domain.Common;

public abstract class TenantBaseEntity : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
}
