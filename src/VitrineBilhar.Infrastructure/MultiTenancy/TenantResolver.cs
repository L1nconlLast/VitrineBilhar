using Microsoft.EntityFrameworkCore;
using VitrineBilhar.Application.Abstractions;
using VitrineBilhar.Infrastructure.Persistence;

namespace VitrineBilhar.Infrastructure.MultiTenancy;

public sealed class TenantResolver(ApplicationDbContext dbContext) : ITenantResolver
{
    public async Task<TenantResolutionResult?> ResolveBySlugAsync(string tenantSlug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tenantSlug))
        {
            return null;
        }

        var normalizedSlug = tenantSlug.Trim().ToLowerInvariant();

        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(t => t.Slug == normalizedSlug)
            .Select(t => new { t.Id, t.Slug })
            .FirstOrDefaultAsync(cancellationToken);

        return tenant is null ? null : new TenantResolutionResult(tenant.Id, tenant.Slug);
    }
}
