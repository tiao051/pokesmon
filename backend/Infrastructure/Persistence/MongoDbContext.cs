using MongoDB.Driver;
using backend.Core.Entities;

namespace backend.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration["MONGO_URI"]
            ?? throw new InvalidOperationException("MONGO_URI is not configured");
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(new MongoUrl(connectionString).DatabaseName);
    }

    public IMongoCollection<Product> Products => _database.GetCollection<Product>("items");
    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<OtpRecord> RegistrationPins => _database.GetCollection<OtpRecord>("registration_pins");
    public IMongoCollection<OtpRecord> ResetPasswordPins => _database.GetCollection<OtpRecord>("reset_password_pins");
    public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");
}
