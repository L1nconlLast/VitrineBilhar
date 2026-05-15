namespace VitrineBilhar.Infrastructure.Options;

public sealed class MultiTenantOptions
{
    public const string SectionName = "MultiTenant";

    public bool AllowHeaderTenantIdInDevelopment { get; set; }
}
