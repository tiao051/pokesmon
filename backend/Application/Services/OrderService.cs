using backend.Core.Common;
using backend.Core.DTOs;
using backend.Core.Entities;
using backend.Infrastructure.Persistence;
using MongoDB.Driver;

namespace backend.Application.Services;

public enum OrderListView { Active, Completed, All }

public record OrderListQuery(string UserEmail, bool? IsPreorder, OrderListView View);

public enum OrderResultKind { Ok, NotFound, BadRequest }

public class OrderOperationResult<T>
{
    public OrderResultKind Kind { get; init; }
    public string? Error { get; init; }
    public T? Value { get; init; }

    public static OrderOperationResult<T> Success(T value) => new() { Kind = OrderResultKind.Ok, Value = value };
    public static OrderOperationResult<T> NotFound(string error) => new() { Kind = OrderResultKind.NotFound, Error = error };
    public static OrderOperationResult<T> BadRequest(string error) => new() { Kind = OrderResultKind.BadRequest, Error = error };
}

public class OrderService
{
    private readonly MongoDbContext _context;

    public OrderService(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderResponse>> ListAsync(OrderListQuery q, CancellationToken ct = default)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.UserEmail, q.UserEmail);

        if (q.IsPreorder.HasValue)
            filter &= Builders<Order>.Filter.Eq(o => o.IsPreorder, q.IsPreorder.Value);

        if (q.View == OrderListView.Active)
            filter &= Builders<Order>.Filter.In(o => o.Status, OrderStatus.ActiveStatuses);
        else if (q.View == OrderListView.Completed)
            filter &= Builders<Order>.Filter.In(o => o.Status, OrderStatus.CompletedStatuses);

        var orders = await _context.Orders
            .Find(filter)
            .SortByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<OrderOperationResult<OrderResponse>> GetByIdAsync(
        string id, string userEmail, CancellationToken ct = default)
    {
        var order = await _context.Orders.Find(o => o.Id == id).FirstOrDefaultAsync(ct);
        if (order is null) return OrderOperationResult<OrderResponse>.NotFound("Order not found");
        if (order.UserEmail != userEmail) return OrderOperationResult<OrderResponse>.NotFound("Order not found");

        return OrderOperationResult<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<OrderOperationResult<OrderResponse>> CreateAsync(
        string userEmail, CreateOrderDto dto, CancellationToken ct = default)
    {
        if (dto.Items.Count == 0)
            return OrderOperationResult<OrderResponse>.BadRequest("Order must contain at least one item");

        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Find(Builders<Product>.Filter.In(p => p.ProductId, productIds))
            .ToListAsync(ct);

        var items = new List<OrderItem>();
        decimal total = 0m;
        decimal deposit = 0m;

        foreach (var item in dto.Items)
        {
            var product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
            if (product is null)
                return OrderOperationResult<OrderResponse>.BadRequest($"Product {item.ProductId} not found");

            if (dto.IsPreorder && !product.IsPreorder)
                return OrderOperationResult<OrderResponse>.BadRequest(
                    $"Product {product.ProductId} is not available for pre-order");

            var price = product.MarketPrice ?? product.LowestPrice ?? 0m;
            var lineTotal = price * item.Quantity;
            items.Add(new OrderItem
            {
                ProductId = product.ProductId,
                Slug = product.ProductUrlName,
                Title = product.ProductName,
                Price = price,
                Quantity = item.Quantity,
                Image = product.ImageUrl,
            });
            total += lineTotal;

            if (dto.IsPreorder)
                deposit += lineTotal * RarityDepositRates.RateFor(product.RarityName);
        }

        var order = new Order
        {
            UserEmail = userEmail,
            Items = items,
            Total = total,
            IsPreorder = dto.IsPreorder,
            Status = dto.IsPreorder ? OrderStatus.Stocking : OrderStatus.Placed,
            DepositAmount = dto.IsPreorder ? Math.Round(deposit, 2) : null,
            DepositPaid = false,
        };

        await _context.Orders.InsertOneAsync(order, cancellationToken: ct);
        return OrderOperationResult<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<OrderOperationResult<OrderResponse>> UpdateStatusAsync(
        string id, string newStatus, CancellationToken ct = default)
    {
        var order = await _context.Orders.Find(o => o.Id == id).FirstOrDefaultAsync(ct);
        if (order is null) return OrderOperationResult<OrderResponse>.NotFound("Order not found");

        if (!OrderStatus.IsValid(newStatus, order.IsPreorder))
            return OrderOperationResult<OrderResponse>.BadRequest($"Invalid status '{newStatus}' for this order type");

        var update = Builders<Order>.Update
            .Set(o => o.Status, newStatus)
            .Set(o => o.UpdatedAt, DateTime.UtcNow);

        await _context.Orders.UpdateOneAsync(o => o.Id == id, update, cancellationToken: ct);
        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        return OrderOperationResult<OrderResponse>.Success(MapToResponse(order));
    }

    private static OrderResponse MapToResponse(Order o) => new(
        Id: o.Id!,
        UserEmail: o.UserEmail,
        Items: o.Items.Select(i => new OrderItemDto(
            i.ProductId, i.Slug, i.Title, i.Price, i.Quantity, i.Image)).ToList(),
        Total: o.Total,
        Status: o.Status,
        IsPreorder: o.IsPreorder,
        DepositAmount: o.DepositAmount,
        DepositPaid: o.DepositPaid,
        CreatedAt: o.CreatedAt,
        UpdatedAt: o.UpdatedAt
    );
}
