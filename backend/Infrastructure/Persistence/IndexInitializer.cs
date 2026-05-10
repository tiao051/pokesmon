using backend.Core.Common;
using backend.Core.Entities;
using MongoDB.Driver;

namespace backend.Infrastructure.Persistence;

public static class IndexInitializer
{
    public static async Task EnsureIndexesAsync(MongoDbContext ctx, CancellationToken ct = default)
    {
        await ctx.Users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true, Name = "ix_users_email_unique" }),
            cancellationToken: ct);

        await ctx.Products.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(p => p.ProductUrlName),
                new CreateIndexOptions { Name = "ix_products_url" }),
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys
                    .Ascending(p => p.ProductLineName)
                    .Ascending(p => p.ProductTypeName),
                new CreateIndexOptions { Name = "ix_products_line_type" }),
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Descending(p => p.MarketPrice),
                new CreateIndexOptions { Name = "ix_products_price" }),
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Descending(p => p.CreatedAt),
                new CreateIndexOptions { Name = "ix_products_created" }),
        }, cancellationToken: ct);

        await ctx.Orders.Indexes.CreateOneAsync(
            new CreateIndexModel<Order>(
                Builders<Order>.IndexKeys
                    .Ascending(o => o.UserEmail)
                    .Descending(o => o.CreatedAt),
                new CreateIndexOptions { Name = "ix_orders_user_created" }),
            cancellationToken: ct);

        await ctx.Favorites.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Favorite>(
                Builders<Favorite>.IndexKeys
                    .Ascending(f => f.UserEmail)
                    .Ascending(f => f.ProductId),
                new CreateIndexOptions { Unique = true, Name = "ix_favorites_user_product_unique" }),
            new CreateIndexModel<Favorite>(
                Builders<Favorite>.IndexKeys
                    .Ascending(f => f.UserEmail)
                    .Descending(f => f.CreatedAt),
                new CreateIndexOptions { Name = "ix_favorites_user_created" }),
        }, cancellationToken: ct);

        var ttl = TimeSpan.FromMinutes(Constants.OtpExpiryMinutes + 5);
        foreach (var collection in new[] { ctx.RegistrationPins, ctx.ResetPasswordPins })
        {
            await collection.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<OtpRecord>(
                    Builders<OtpRecord>.IndexKeys.Ascending(p => p.Email),
                    new CreateIndexOptions { Name = "ix_otp_email" }),
                new CreateIndexModel<OtpRecord>(
                    Builders<OtpRecord>.IndexKeys.Ascending(p => p.ExpiresAt),
                    new CreateIndexOptions { Name = "ix_otp_ttl", ExpireAfter = ttl }),
            }, cancellationToken: ct);
        }
    }
}
