using backend.Core.DTOs;
using backend.Core.Entities;
using backend.Infrastructure.Persistence;
using MongoDB.Driver;

namespace backend.Application.Services;

public enum FavoriteResultKind { Ok, NotFound, BadRequest }

public class FavoriteOperationResult
{
    public FavoriteResultKind Kind { get; init; }
    public string? Error { get; init; }

    public static FavoriteOperationResult Success() => new() { Kind = FavoriteResultKind.Ok };
    public static FavoriteOperationResult NotFound(string error) => new() { Kind = FavoriteResultKind.NotFound, Error = error };
    public static FavoriteOperationResult BadRequest(string error) => new() { Kind = FavoriteResultKind.BadRequest, Error = error };
}

public class FavoritesService
{
    private readonly MongoDbContext _context;
    private readonly ProductService _productService;

    public FavoritesService(MongoDbContext context, ProductService productService)
    {
        _context = context;
        _productService = productService;
    }

    public async Task<List<FavoriteResponse>> ListAsync(string userEmail, CancellationToken ct = default)
    {
        var favorites = await _context.Favorites
            .Find(f => f.UserEmail == userEmail)
            .SortByDescending(f => f.CreatedAt)
            .ToListAsync(ct);

        if (favorites.Count == 0) return new List<FavoriteResponse>();

        var products = await _productService.GetByProductIdsAsync(
            favorites.Select(f => f.ProductId), ct);
        var productById = products.ToDictionary(p => p.Id);

        return favorites
            .Where(f => productById.ContainsKey(f.ProductId))
            .Select(f => new FavoriteResponse(f.ProductId, f.CreatedAt, productById[f.ProductId]))
            .ToList();
    }

    public async Task<List<int>> ListIdsAsync(string userEmail, CancellationToken ct = default)
    {
        var favorites = await _context.Favorites
            .Find(f => f.UserEmail == userEmail)
            .Project(f => f.ProductId)
            .ToListAsync(ct);
        return favorites;
    }

    public async Task<FavoriteOperationResult> AddAsync(
        string userEmail, int productId, CancellationToken ct = default)
    {
        var productExists = await _context.Products
            .Find(p => p.ProductId == productId)
            .AnyAsync(ct);
        if (!productExists)
            return FavoriteOperationResult.BadRequest($"Product {productId} not found");

        var existing = await _context.Favorites
            .Find(f => f.UserEmail == userEmail && f.ProductId == productId)
            .AnyAsync(ct);
        if (existing) return FavoriteOperationResult.Success();

        await _context.Favorites.InsertOneAsync(new Favorite
        {
            UserEmail = userEmail,
            ProductId = productId,
        }, cancellationToken: ct);

        return FavoriteOperationResult.Success();
    }

    public async Task<FavoriteOperationResult> RemoveAsync(
        string userEmail, int productId, CancellationToken ct = default)
    {
        var result = await _context.Favorites.DeleteOneAsync(
            f => f.UserEmail == userEmail && f.ProductId == productId,
            cancellationToken: ct);

        return result.DeletedCount > 0
            ? FavoriteOperationResult.Success()
            : FavoriteOperationResult.NotFound("Favorite not found");
    }
}
