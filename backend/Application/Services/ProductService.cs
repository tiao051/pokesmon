using System.Text.RegularExpressions;
using backend.Core.Common;
using backend.Core.DTOs;
using backend.Core.Entities;
using backend.Infrastructure.Persistence;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;

namespace backend.Application.Services;

public record ProductListQuery(
    string? ProductType,
    string? SetName,
    string? Search,
    decimal? MaxPrice,
    bool? Sealed,
    bool? IsPreorder,
    string? Sort,
    int Page,
    int Limit
);

public class ProductService
{
    private const string CategoriesCacheKey = "products:categories";
    private static readonly TimeSpan CategoriesCacheTtl = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan ListCacheTtl = TimeSpan.FromSeconds(60);

    private readonly MongoDbContext _context;
    private readonly IMemoryCache _cache;

    public ProductService(MongoDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<ProductListResponse> ListAsync(ProductListQuery q, CancellationToken ct = default)
    {
        var cacheKey = BuildListCacheKey(q);
        if (_cache.TryGetValue(cacheKey, out ProductListResponse? cached) && cached is not null)
            return cached;

        var filter = BuildFilter(q);
        var sort = q.Sort switch
        {
            "newest" => Builders<Product>.Sort.Descending(p => p.CreatedAt),
            _ => Builders<Product>.Sort.Descending(p => p.MarketPrice),
        };

        var total = await _context.Products.CountDocumentsAsync(filter, cancellationToken: ct);
        var products = await _context.Products
            .Find(filter)
            .Sort(sort)
            .Skip((q.Page - 1) * q.Limit)
            .Limit(q.Limit)
            .ToListAsync(ct);

        var items = products.Select(p => MapToResponse(p, includeBase64: false)).ToList();
        var result = new ProductListResponse(items, total, q.Page, q.Limit);
        _cache.Set(cacheKey, result, ListCacheTtl);
        return result;
    }

    private static string BuildListCacheKey(ProductListQuery q) =>
        $"products:list:{q.ProductType}:{q.SetName}:{q.Search}:{q.MaxPrice}:{q.Sealed}:{q.IsPreorder}:{q.Sort}:{q.Page}:{q.Limit}";

    public async Task<ProductDetailResponse?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var product = await _context.Products
            .Find(p => p.ProductUrlName == slug && p.ProductLineName == Constants.PokemonProductLine)
            .FirstOrDefaultAsync(ct);

        if (product is null) return null;

        var related = await _context.Products
            .Find(p => p.ProductTypeName == product.ProductTypeName
                       && p.ProductUrlName != slug
                       && p.ProductLineName == Constants.PokemonProductLine)
            .SortByDescending(p => p.MarketPrice)
            .Limit(4)
            .ToListAsync(ct);

        return new ProductDetailResponse(
            MapToResponse(product, includeBase64: true),
            related.Select(p => MapToResponse(p, includeBase64: false)).ToList());
    }

    public async Task<List<ProductResponse>> GetByProductIdsAsync(
        IEnumerable<int> productIds, CancellationToken ct = default)
    {
        var ids = productIds.Distinct().ToList();
        if (ids.Count == 0) return new List<ProductResponse>();

        var products = await _context.Products
            .Find(Builders<Product>.Filter.In(p => p.ProductId, ids))
            .ToListAsync(ct);

        return products.Select(p => MapToResponse(p, includeBase64: false)).ToList();
    }

    public async Task<CategoriesResponse> GetCategoriesAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CategoriesCacheKey, out CategoriesResponse? cached) && cached is not null)
            return cached;

        var baseFilter = Builders<Product>.Filter.Eq(p => p.ProductLineName, Constants.PokemonProductLine);
        var typeCursor = await _context.Products.DistinctAsync(p => p.ProductTypeName, baseFilter, cancellationToken: ct);
        var setCursor = await _context.Products.DistinctAsync(p => p.SetName, baseFilter, cancellationToken: ct);

        var productTypes = (await typeCursor.ToListAsync(ct))
            .Where(t => !string.IsNullOrWhiteSpace(t)).OrderBy(t => t).ToList();
        var setNames = (await setCursor.ToListAsync(ct))
            .Where(s => !string.IsNullOrWhiteSpace(s)).OrderBy(s => s).ToList();

        var result = new CategoriesResponse(productTypes, setNames);
        _cache.Set(CategoriesCacheKey, result, CategoriesCacheTtl);
        return result;
    }

    private static FilterDefinition<Product> BuildFilter(ProductListQuery q)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.ProductLineName, Constants.PokemonProductLine);

        if (!string.IsNullOrWhiteSpace(q.ProductType))
            filter &= Builders<Product>.Filter.Eq(p => p.ProductTypeName, q.ProductType);

        if (!string.IsNullOrWhiteSpace(q.SetName))
            filter &= Builders<Product>.Filter.Eq(p => p.SetName, q.SetName);

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var pattern = new BsonRegularExpression(Regex.Escape(q.Search), "i");
            filter &= Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Regex(p => p.ProductName, pattern),
                Builders<Product>.Filter.Regex(p => p.SetName, pattern),
                Builders<Product>.Filter.Regex(p => p.ProductTypeName, pattern));
        }

        if (q.MaxPrice.HasValue)
            filter &= Builders<Product>.Filter.Lte(p => p.MarketPrice, q.MaxPrice.Value);

        if (q.Sealed.HasValue)
            filter &= Builders<Product>.Filter.Eq(p => p.Sealed, q.Sealed.Value);

        if (q.IsPreorder.HasValue)
            filter &= Builders<Product>.Filter.Eq(p => p.IsPreorder, q.IsPreorder.Value);

        return filter;
    }

    private static ProductResponse MapToResponse(Product p, bool includeBase64)
    {
        var stock = p.Listings > 0 ? p.Listings : 1;
        var description = $"{p.ProductTypeName} from the {p.SetName} expansion."
                          + (p.Sealed ? " Factory sealed." : "")
                          + (!string.IsNullOrWhiteSpace(p.RarityName) ? $" Rarity: {p.RarityName}." : "");

        var tags = new List<string>();
        if (p.Sealed) tags.Add("sealed");
        if (p.IsPreorder) tags.Add("preorder");

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
            IsPreorder: p.IsPreorder,
            Tags: tags
        );
    }
}
