using Microsoft.AspNetCore.Mvc;
using backend.Core.Entities;
using backend.Core.DTOs;
using MongoDB.Driver;
using backend.Infrastructure.Persistence;

namespace backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly MongoDbContext _context;

    public ProductsController(MongoDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// List products with optional filters: productType, setName, search, page, limit.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetProducts(
        [FromQuery] string? productType = null,
        [FromQuery] string? setName = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.ProductLineName, "Pokemon");

        if (!string.IsNullOrWhiteSpace(productType))
            filter &= Builders<Product>.Filter.Eq(p => p.ProductTypeName, productType);

        if (!string.IsNullOrWhiteSpace(setName))
            filter &= Builders<Product>.Filter.Eq(p => p.SetName, setName);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLowerInvariant();
            filter &= Builders<Product>.Filter.Where(p =>
                p.ProductName.ToLower().Contains(searchLower) ||
                p.SetName.ToLower().Contains(searchLower) ||
                p.ProductTypeName.ToLower().Contains(searchLower));
        }

        var total = await _context.Products.CountDocumentsAsync(filter);

        var products = await _context.Products
            .Find(filter)
            .SortByDescending(p => p.MarketPrice)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();

        var items = products.Select(MapToResponse).ToList();

        return Ok(new { items, total, page, limit });
    }

    /// <summary>
    /// Get a single product by its URL slug (productUrlName).
    /// </summary>
    [HttpGet("{slug}")]
    public async Task<ActionResult> GetProduct(string slug)
    {
        var product = await _context.Products
            .Find(p => p.ProductUrlName == slug && p.ProductLineName == "Pokemon")
            .FirstOrDefaultAsync();

        if (product == null) return NotFound(new { error = "Product not found" });

        // Related: same productTypeName, exclude current
        var related = await _context.Products
            .Find(p => p.ProductTypeName == product.ProductTypeName
                       && p.ProductUrlName != slug
                       && p.ProductLineName == "Pokemon")
            .SortByDescending(p => p.MarketPrice)
            .Limit(4)
            .ToListAsync();

        return Ok(new
        {
            product = MapToResponse(product),
            related = related.Select(MapToResponse).ToList()
        });
    }

    /// <summary>
    /// Returns distinct product types and set names from the Pokémon product collection.
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult> GetCategories()
    {
        var products = await _context.Products
            .Find(p => p.ProductLineName == "Pokemon")
            .ToListAsync();

        var productTypes = products
            .Select(p => p.ProductTypeName)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        var setNames = products
            .Select(p => p.SetName)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        return Ok(new { productTypes, setNames });
    }

    private static ProductResponse MapToResponse(Product p)
    {
        var stock = p.Listings > 0 ? p.Listings : 1;
        var description = $"{p.ProductTypeName} from the {p.SetName} expansion."
                          + (p.Sealed ? " Factory sealed." : "")
                          + (!string.IsNullOrWhiteSpace(p.RarityName) ? $" Rarity: {p.RarityName}." : "");

        return new ProductResponse(
            Id: p.ProductId,
            Slug: p.ProductUrlName,
            Title: p.ProductName,
            Price: p.MarketPrice ?? p.LowestPrice ?? 0m,
            Image: string.IsNullOrWhiteSpace(p.ImageUrl)
                ? $"https://placehold.co/400x500/003153/FFC512?text={Uri.EscapeDataString(p.ProductName)}"
                : p.ImageUrl,
            ImageBase64: p.ImageBase64,
            Description: description,
            Stock: stock,
            Category: p.ProductTypeName,
            ProductTypeName: p.ProductTypeName,
            SetName: p.SetName,
            RarityName: p.RarityName,
            Sealed: p.Sealed,
            Tags: p.Sealed ? new List<string> { "sealed" } : new List<string>()
        );
    }
}
