using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Core.Entities;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userEmail")]
    public string UserEmail { get; set; } = string.Empty;

    [BsonElement("items")]
    public List<OrderItem> Items { get; set; } = new();

    [BsonElement("total")]
    public decimal Total { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = OrderStatus.Placed;

    [BsonElement("isPreorder")]
    public bool IsPreorder { get; set; }

    [BsonElement("depositAmount")]
    public decimal? DepositAmount { get; set; }

    [BsonElement("depositPaid")]
    public bool DepositPaid { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class OrderItem
{
    [BsonElement("productId")]
    public int ProductId { get; set; }

    [BsonElement("slug")]
    public string Slug { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("image")]
    public string Image { get; set; } = string.Empty;
}

public static class OrderStatus
{
    public const string Stocking = "Stocking";
    public const string InWarehouse = "InWarehouse";
    public const string Placed = "Placed";
    public const string Shipping = "Shipping";
    public const string Delivered = "Delivered";
    public const string Cancelled = "Cancelled";
    public const string Refunded = "Refunded";

    public static readonly string[] ActiveStatuses =
        { Stocking, InWarehouse, Placed, Shipping };

    public static readonly string[] CompletedStatuses =
        { Delivered, Cancelled, Refunded };

    public static readonly string[] PreorderFlow =
        { Stocking, InWarehouse, Placed, Shipping, Delivered };

    public static readonly string[] NormalFlow =
        { Placed, Shipping, Delivered };

    public static bool IsValid(string status, bool isPreorder) =>
        isPreorder
            ? PreorderFlow.Contains(status) || status == Cancelled || status == Refunded
            : NormalFlow.Contains(status) || status == Cancelled || status == Refunded;
}
