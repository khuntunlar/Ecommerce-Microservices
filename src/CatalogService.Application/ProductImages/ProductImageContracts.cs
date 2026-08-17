namespace CatalogService.Application.ProductImages;

public sealed record ProductImageRequest(
    string Url,
    string AltText,
    int SortOrder = 0,
    bool IsPrimary = false);

public sealed record ProductImageDto(
    Guid Id,
    Guid ProductId,
    string Url,
    string AltText,
    int SortOrder,
    bool IsPrimary);
