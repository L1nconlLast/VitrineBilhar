namespace VitrineBilhar.Application.Abstractions;

public interface ITenantResolver
{
    Task<TenantResolutionResult?> ResolveBySlugAsync(string tenantSlug, CancellationToken cancellationToken = default);
}

public sealed record TenantResolutionResult(Guid TenantId, string TenantSlug);
