using backend.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult> GetProducts(
        [FromQuery] string? productType,
        [FromQuery] string? setName,
        [FromQuery] string? search,
        [FromQuery] decimal? maxPrice,
        [FromQuery(Name = "sealed")] bool? isSealed,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        CancellationToken ct = default)
    {
        if (page < 1 || limit < 1 || limit > 100)
            return BadRequest(new { error = "Invalid page or limit" });

        var query = new ProductListQuery(productType, setName, search, maxPrice, isSealed, sort, page, limit);
        var result = await _service.ListAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult> GetProduct(string slug, CancellationToken ct)
    {
        var detail = await _service.GetBySlugAsync(slug, ct);
        if (detail is null) return NotFound(new { error = "Product not found" });
        return Ok(detail);
    }

    [HttpGet("categories")]
    public async Task<ActionResult> GetCategories(CancellationToken ct)
    {
        var result = await _service.GetCategoriesAsync(ct);
        return Ok(result);
    }
}
