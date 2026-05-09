using backend.Core.Entities;
using MongoDB.Driver;

namespace backend.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(MongoDbContext context, IConfiguration config)
    {
        var seedEmail = config["SEED_ADMIN_EMAIL"];
        var seedPassword = config["SEED_ADMIN_PASSWORD"];

        if (string.IsNullOrWhiteSpace(seedEmail) || string.IsNullOrWhiteSpace(seedPassword))
            return;

        await SeedAdminUserAsync(context, seedEmail, seedPassword);
        await SeedSampleOrdersAsync(context, seedEmail);
    }

    private static async Task SeedAdminUserAsync(MongoDbContext context, string email, string password)
    {
        if (await context.Users.Find(u => u.Email == email).AnyAsync()) return;

        await context.Users.InsertOneAsync(new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsVerified = true
        });
    }

    private static async Task SeedSampleOrdersAsync(MongoDbContext context, string email)
    {
        if (await context.Orders.Find(o => o.UserEmail == email).AnyAsync()) return;

        var products = await context.Products
            .Find(p => p.ProductLineName == "Pokemon")
            .Limit(8)
            .ToListAsync();
        if (products.Count == 0) return;

        var now = DateTime.UtcNow;
        var orders = new List<Order>
        {
            BuildOrder(email, products.Take(2), isPreorder: false, OrderStatus.Placed, now.AddDays(-1)),
            BuildOrder(email, products.Skip(1).Take(2), isPreorder: false, OrderStatus.Shipping, now.AddDays(-3)),
            BuildOrder(email, products.Skip(2).Take(1), isPreorder: false, OrderStatus.Delivered, now.AddDays(-10)),
            BuildOrder(email, products.Skip(3).Take(1), isPreorder: false, OrderStatus.Cancelled, now.AddDays(-12)),
            BuildOrder(email, products.Skip(4).Take(2), isPreorder: true, OrderStatus.Stocking, now.AddDays(-2)),
            BuildOrder(email, products.Skip(5).Take(1), isPreorder: true, OrderStatus.InWarehouse, now.AddDays(-6)),
            BuildOrder(email, products.Skip(6).Take(1), isPreorder: true, OrderStatus.Shipping, now.AddDays(-9)),
            BuildOrder(email, products.Skip(7).Take(1), isPreorder: true, OrderStatus.Delivered, now.AddDays(-20)),
        };

        await context.Orders.InsertManyAsync(orders);
    }

    private static Order BuildOrder(
        string email,
        IEnumerable<Product> products,
        bool isPreorder,
        string status,
        DateTime createdAt)
    {
        var items = products.Select(p => new OrderItem
        {
            ProductId = p.ProductId,
            Slug = p.ProductUrlName,
            Title = p.ProductName,
            Price = p.MarketPrice ?? p.LowestPrice ?? 0m,
            Quantity = 1,
            Image = p.ImageUrl,
        }).ToList();

        var total = items.Sum(i => i.Price * i.Quantity);
        return new Order
        {
            UserEmail = email,
            Items = items,
            Total = total,
            Status = status,
            IsPreorder = isPreorder,
            DepositAmount = isPreorder ? Math.Round(total * 0.2m, 2) : null,
            DepositPaid = isPreorder && status != OrderStatus.Stocking,
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
        };
    }
}
