namespace CatalogService.Application.Categories;

public sealed record CategoryRequest(string Name, string Slug, bool IsActive = true);
public sealed record CategoryDto(Guid Id, string Name, string Slug, bool IsActive);
