using System.ComponentModel.DataAnnotations;

namespace backend.Core.DTOs;

public record OrderItemDto(
    int ProductId,
    string Slug,
    string Title,
    decimal Price,
    int Quantity,
    string Image
);

public record OrderResponse(
    string Id,
    string UserEmail,
    List<OrderItemDto> Items,
    decimal Total,
    string Status,
    bool IsPreorder,
    decimal? DepositAmount,
    bool DepositPaid,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateOrderItemDto(
    [Required] int ProductId,
    [Required, Range(1, 999)] int Quantity
);

public record CreateOrderDto(
    [Required, MinLength(1)] List<CreateOrderItemDto> Items,
    bool IsPreorder
);

public record UpdateOrderStatusDto(
    [Required] string Status
);
