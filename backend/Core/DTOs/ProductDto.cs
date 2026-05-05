namespace backend.Core.DTOs;

public record ProductDto(int Id, string Name, string Description, decimal Price, string ImageUrl);
public record CreateProductDto(string Name, string Description, decimal Price, string ImageUrl);
