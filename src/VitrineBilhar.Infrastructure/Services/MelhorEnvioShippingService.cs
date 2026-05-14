using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitrineBilhar.Domain.Shipping;
using VitrineBilhar.Infrastructure.Options;

namespace VitrineBilhar.Infrastructure.Services;

public sealed class MelhorEnvioShippingService(
    IHttpClientFactory httpClientFactory,
    IOptions<MelhorEnvioOptions> options,
    ILogger<MelhorEnvioShippingService> logger) : IShippingService
{
    private const decimal StubRate = 10m;

    public async Task<decimal> CalculateAsync(Guid tenantId, string destinationZipCode, decimal orderTotal, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("MelhorEnvio");

        if (!string.IsNullOrWhiteSpace(options.Value.Token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.Token);
        }

        try
        {
            using var response = await client.GetAsync("api/v2/me/shipment/companies", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Melhor Envio retornou status {StatusCode}", response.StatusCode);
                return 0m;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var doc = JsonDocument.Parse(content);
            return doc.RootElement.ValueKind == JsonValueKind.Array ? StubRate : 0m;
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Falha ao consultar Melhor Envio.");
            return 0m;
        }
    }
}
