using VitrineBilhar.Domain.Shipping;

namespace VitrineBilhar.Infrastructure.Services;

public sealed class HybridShippingService(
    MelhorEnvioShippingService melhorEnvioShippingService,
    FixedRateShippingService fixedRateShippingService) : IShippingService
{
    public async Task<decimal> CalculateAsync(Guid tenantId, string destinationZipCode, decimal orderTotal, CancellationToken cancellationToken = default)
    {
        var melhorEnvioRate = await melhorEnvioShippingService.CalculateAsync(tenantId, destinationZipCode, orderTotal, cancellationToken);
        if (melhorEnvioRate > 0)
        {
            return melhorEnvioRate;
        }

        return await fixedRateShippingService.CalculateAsync(tenantId, destinationZipCode, orderTotal, cancellationToken);
    }
}
