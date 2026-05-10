using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Core.Entities;

public class Favorite
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userEmail")]
    public string UserEmail { get; set; } = string.Empty;

    [BsonElement("productId")]
    public int ProductId { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
