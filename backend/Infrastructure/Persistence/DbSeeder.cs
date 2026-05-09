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

        if (await context.Users.Find(u => u.Email == seedEmail).AnyAsync())
            return;

        var user = new User
        {
            Email = seedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedPassword),
            IsVerified = true
        };
        await context.Users.InsertOneAsync(user);
    }
}
