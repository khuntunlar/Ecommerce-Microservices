namespace CatalogService.Application.Brands;

public sealed record BrandRequest(string Name, string Slug, bool IsActive = true);
public sealed record BrandDto(Guid Id, string Name, string Slug, bool IsActive);
