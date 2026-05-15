namespace VitrineBilhar.Application.Products;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> ListAdminAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(UpsertProductRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpsertProductRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductDto>> ListCatalogBySlugAsync(string tenantSlug, CancellationToken cancellationToken = default);
}
