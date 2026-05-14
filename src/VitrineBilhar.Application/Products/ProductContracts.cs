namespace VitrineBilhar.Application.Products;

public sealed record ProductDto(Guid Id, string Name, string? Description, decimal Price, bool IsActive, Guid? CategoryId);

public sealed record UpsertProductRequest(string Name, string? Description, decimal Price, bool IsActive, Guid? CategoryId);
