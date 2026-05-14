using Microsoft.AspNetCore.Mvc;
using VitrineBilhar.Application.Products;

namespace VitrineBilhar.API.Controllers;

[ApiController]
[Route("api/catalog")]
public sealed class CatalogController(IProductService productService) : ControllerBase
{
    [HttpGet("{tenantSlug}/products")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> ListProducts(string tenantSlug, CancellationToken cancellationToken)
    {
        var products = await productService.ListCatalogBySlugAsync(tenantSlug, cancellationToken);
        return Ok(products);
    }
}
