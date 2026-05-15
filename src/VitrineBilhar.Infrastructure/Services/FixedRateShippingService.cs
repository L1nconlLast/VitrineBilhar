using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VitrineBilhar.Domain.Shipping;
using VitrineBilhar.Infrastructure.Persistence;

namespace VitrineBilhar.Infrastructure.Services;

public sealed class FixedRateShippingService(
    ApplicationDbContext dbContext,
    IHttpClientFactory httpClientFactory) : IShippingService
{
    public async Task<decimal> CalculateAsync(Guid tenantId, string destinationZipCode, decimal orderTotal, CancellationToken cancellationToken = default)
    {
        var normalizedZipCode = new string(destinationZipCode.Where(char.IsDigit).ToArray());

        _ = await TryGetStateCodeAsync(normalizedZipCode, cancellationToken);

        var zone = await dbContext.ShippingZones
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(z => z.TenantId == tenantId && string.Compare(z.ZipCodeStart, normalizedZipCode, StringComparison.Ordinal) <= 0 && string.Compare(z.ZipCodeEnd, normalizedZipCode, StringComparison.Ordinal) >= 0)
            .OrderBy(z => z.FixedRate)
            .FirstOrDefaultAsync(cancellationToken);

        return zone?.FixedRate ?? 0m;
    }

    private async Task<string?> TryGetStateCodeAsync(string normalizedZipCode, CancellationToken cancellationToken)
    {
        if (normalizedZipCode.Length != 8)
        {
            return null;
        }

        var client = httpClientFactory.CreateClient("ViaCep");

        try
        {
            using var response = await client.GetAsync($"ws/{normalizedZipCode}/json/", cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var data = await JsonSerializer.DeserializeAsync<ViaCepResponse>(stream, cancellationToken: cancellationToken);
            return data?.Uf;
        }
        catch
        {
            return null;
        }
    }

    private sealed class ViaCepResponse
    {
        public string? Uf { get; set; }
    }
}
