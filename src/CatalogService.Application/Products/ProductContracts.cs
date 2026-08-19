namespace CatalogService.Application.Products;

public sealed record ProductRequest(
    Guid CategoryId,
    Guid BrandId,
    string Name,
    string Slug,
    string Description,
    decimal Price,
    string Sku,
    bool IsActive = true);

public sealed record ProductDto(
    Guid Id,
    Guid CategoryId,
    Guid BrandId,
    string Name,
    string Slug,
    string Description,
    decimal Price,
    string Sku,
    bool IsActive);
