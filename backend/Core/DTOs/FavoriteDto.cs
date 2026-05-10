using System.ComponentModel.DataAnnotations;

namespace backend.Core.DTOs;

public record FavoriteResponse(
    int ProductId,
    DateTime CreatedAt,
    ProductResponse Product
);

public record AddFavoriteDto([Required] int ProductId);
