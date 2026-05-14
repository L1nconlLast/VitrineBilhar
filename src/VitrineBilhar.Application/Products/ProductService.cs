using Microsoft.EntityFrameworkCore;
using VitrineBilhar.Application.Abstractions;
using VitrineBilhar.Domain.Entities;

namespace VitrineBilhar.Application.Products;

public sealed class ProductService(
    IApplicationDbContext dbContext,
    ITenantContext tenantContext,
    ITenantResolver tenantResolver) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> ListAdminAsync(CancellationToken cancellationToken = default)
        => await dbContext.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(MapExpression())
            .ToListAsync(cancellationToken);

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(MapExpression())
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ProductDto> CreateAsync(UpsertProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            IsActive = request.IsActive,
            CategoryId = request.CategoryId
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map(product);
    }

    public async Task<bool> UpdateAsync(Guid id, UpsertProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        product.Name = request.Name.Trim();
        product.Description = request.Description?.Trim();
        product.Price = request.Price;
        product.IsActive = request.IsActive;
        product.CategoryId = request.CategoryId;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ProductDto>> ListCatalogBySlugAsync(string tenantSlug, CancellationToken cancellationToken = default)
    {
        var tenant = await tenantResolver.ResolveBySlugAsync(tenantSlug, cancellationToken);
        if (tenant is null)
        {
            return [];
        }

        tenantContext.SetTenant(tenant.TenantId, tenant.TenantSlug);

        try
        {
            return await dbContext.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .Select(MapExpression())
                .ToListAsync(cancellationToken);
        }
        finally
        {
            tenantContext.Clear();
        }
    }

    private static ProductDto Map(Product product)
        => new(product.Id, product.Name, product.Description, product.Price, product.IsActive, product.CategoryId);

    private static System.Linq.Expressions.Expression<Func<Product, ProductDto>> MapExpression()
        => product => new ProductDto(product.Id, product.Name, product.Description, product.Price, product.IsActive, product.CategoryId);
}
