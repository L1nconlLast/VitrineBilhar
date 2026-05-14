namespace VitrineBilhar.Domain.Shipping;

public interface IShippingService
{
    Task<decimal> CalculateAsync(Guid tenantId, string destinationZipCode, decimal orderTotal, CancellationToken cancellationToken = default);
}
