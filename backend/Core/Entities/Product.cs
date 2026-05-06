using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Core.Entities;

[BsonIgnoreExtraElements]
public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("productId")]
    public int ProductId { get; set; }

    [BsonElement("productName")]
    public string ProductName { get; set; } = string.Empty;

    [BsonElement("productUrlName")]
    public string ProductUrlName { get; set; } = string.Empty;

    [BsonElement("productLineName")]
    public string ProductLineName { get; set; } = string.Empty;

    [BsonElement("productLineId")]
    public int ProductLineId { get; set; }

    [BsonElement("productLineUrlName")]
    public string ProductLineUrlName { get; set; } = string.Empty;

    [BsonElement("productTypeName")]
    public string ProductTypeName { get; set; } = string.Empty;

    [BsonElement("productTypeId")]
    public int ProductTypeId { get; set; }

    [BsonElement("rarityName")]
    public string RarityName { get; set; } = string.Empty;

    [BsonElement("sealed")]
    public bool Sealed { get; set; }

    [BsonElement("listings")]
    public int Listings { get; set; }

    [BsonElement("marketPrice")]
    public decimal? MarketPrice { get; set; }

    [BsonElement("lowestPrice")]
    public decimal? LowestPrice { get; set; }

    [BsonElement("medianPrice")]
    public decimal? MedianPrice { get; set; }

    [BsonElement("setName")]
    public string SetName { get; set; } = string.Empty;

    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [BsonElement("imageBase64")]
    public string? ImageBase64 { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
