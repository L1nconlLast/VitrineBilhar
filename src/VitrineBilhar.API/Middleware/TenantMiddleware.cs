using System.Security.Claims;
using Microsoft.Extensions.Options;
using VitrineBilhar.Application.Abstractions;
using VitrineBilhar.Infrastructure.Options;

namespace VitrineBilhar.API.Middleware;

public sealed class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext httpContext,
        ITenantContext tenantContext,
        ITenantResolver tenantResolver,
        IOptions<MultiTenantOptions> options,
        IWebHostEnvironment environment)
    {
        tenantContext.Clear();

        if (TryResolveFromClaims(httpContext.User, out var tenantId, out var tenantSlug))
        {
            tenantContext.SetTenant(tenantId, tenantSlug);
            await next(httpContext);
            return;
        }

        var subdomain = TryGetSubdomain(httpContext.Request.Host.Host);
        if (!string.IsNullOrWhiteSpace(subdomain))
        {
            var tenant = await tenantResolver.ResolveBySlugAsync(subdomain, httpContext.RequestAborted);
            if (tenant is not null)
            {
                tenantContext.SetTenant(tenant.TenantId, tenant.TenantSlug);
                await next(httpContext);
                return;
            }
        }

        if (environment.IsDevelopment() && options.Value.AllowHeaderTenantIdInDevelopment)
        {
            if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var headerValue)
                && Guid.TryParse(headerValue.ToString(), out var headerTenantId))
            {
                var headerSlug = httpContext.Request.Headers["X-Tenant-Slug"].ToString();
                tenantContext.SetTenant(headerTenantId, string.IsNullOrWhiteSpace(headerSlug) ? null : headerSlug);
            }
        }

        await next(httpContext);
    }

    private static bool TryResolveFromClaims(ClaimsPrincipal principal, out Guid tenantId, out string? tenantSlug)
    {
        tenantId = Guid.Empty;
        tenantSlug = null;

        if (principal?.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var tenantIdValue = principal.FindFirstValue("tenant_id");
        tenantSlug = principal.FindFirstValue("tenant_slug");

        return Guid.TryParse(tenantIdValue, out tenantId);
    }

    private static string? TryGetSubdomain(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            return null;
        }

        if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
            || host.StartsWith("localhost:", StringComparison.OrdinalIgnoreCase)
            || host.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var parts = host.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length < 3)
        {
            return null;
        }

        var subdomain = parts[0].ToLowerInvariant();
        return subdomain == "www" ? null : subdomain;
    }
}
