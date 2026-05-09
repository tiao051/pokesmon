using backend.Core.Entities;
using MongoDB.Driver;

namespace backend.Infrastructure.Persistence;

public static class DbSeeder
{
    public const string SeedEmail = "admin@pokemon.local";
    public const string SeedPassword = "Admin123!";

    public static async Task SeedAsync(MongoDbContext context)
    {
        if (await context.Users.Find(u => u.Email == SeedEmail).AnyAsync())
        {
            return;
        }

        var user = new User
        {
            Email = SeedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(SeedPassword),
            IsVerified = true
        };

        await context.Users.InsertOneAsync(user);
    }
}
