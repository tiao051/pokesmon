namespace backend.Core.DTOs;

public record ProductResponse(
    int Id,
    string Slug,
    string Title,
    decimal Price,
    string Image,
    string? ImageBase64,
    string Description,
    int Stock,
    string Category,
    string ProductTypeName,
    string SetName,
    string RarityName,
    bool Sealed,
    List<string> Tags
);

public record ProductListResponse(
    List<ProductResponse> Items,
    long Total,
    int Page,
    int Limit
);

public record CategoriesResponse(List<string> ProductTypes, List<string> SetNames);

public record ProductDetailResponse(ProductResponse Product, List<ProductResponse> Related);
