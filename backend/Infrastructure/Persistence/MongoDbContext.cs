using MongoDB.Driver;
using backend.Core.Entities;

namespace backend.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration["MONGO_URI"] ?? "mongodb://localhost:27017/tcgplayer";
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(new MongoUrl(connectionString).DatabaseName);
    }

    public IMongoCollection<Product> Products => _database.GetCollection<Product>("items");
    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<RegistrationPin> RegistrationPins => _database.GetCollection<RegistrationPin>("registration_pins");
    public IMongoCollection<ResetPasswordPin> ResetPasswordPins => _database.GetCollection<ResetPasswordPin>("reset_password_pins");
}
