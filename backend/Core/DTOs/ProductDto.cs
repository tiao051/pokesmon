namespace backend.Core.DTOs;

public record ProductDto(int Id, string Name, string Description, decimal Price, string ImageUrl);

public record CreateProductDto(string Name, string Description, decimal Price, string ImageUrl);

/// <summary>
/// API response shape matching what the frontend expects.
/// </summary>
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
