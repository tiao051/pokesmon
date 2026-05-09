using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using backend.Core.Entities;
using backend.Core.DTOs;
using MongoDB.Bson;
using MongoDB.Driver;
using backend.Infrastructure.Persistence;
using System.Text.RegularExpressions;

namespace backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly MongoDbContext _context;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CategoriesCacheTtl = TimeSpan.FromMinutes(10);
    private const string CategoriesCacheKey = "products:categories";

    public ProductsController(MongoDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
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
            var pattern = new BsonRegularExpression(Regex.Escape(search), "i");
            filter &= Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Regex(p => p.ProductName, pattern),
                Builders<Product>.Filter.Regex(p => p.SetName, pattern),
                Builders<Product>.Filter.Regex(p => p.ProductTypeName, pattern));
        }

        var total = await _context.Products.CountDocumentsAsync(filter);

        var products = await _context.Products
            .Find(filter)
            .SortByDescending(p => p.MarketPrice)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();

        var items = products.Select(p => MapToResponse(p, includeBase64: false)).ToList();

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
            product = MapToResponse(product, includeBase64: true),
            related = related.Select(p => MapToResponse(p, includeBase64: false)).ToList()
        });
    }

    /// <summary>
    /// Returns distinct product types and set names from the Pokémon product collection.
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult> GetCategories()
    {
        if (_cache.TryGetValue(CategoriesCacheKey, out object? cached) && cached is not null)
            return Ok(cached);

        var baseFilter = Builders<Product>.Filter.Eq(p => p.ProductLineName, "Pokemon");

        var typeCursor = await _context.Products.DistinctAsync(p => p.ProductTypeName, baseFilter);
        var setCursor = await _context.Products.DistinctAsync(p => p.SetName, baseFilter);

        var productTypes = (await typeCursor.ToListAsync())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .OrderBy(t => t)
            .ToList();

        var setNames = (await setCursor.ToListAsync())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .OrderBy(s => s)
            .ToList();

        var result = new { productTypes, setNames };
        _cache.Set(CategoriesCacheKey, result, CategoriesCacheTtl);

        return Ok(result);
    }

    private static ProductResponse MapToResponse(Product p, bool includeBase64)
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
            ImageBase64: includeBase64 ? p.ImageBase64 : null,
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
