namespace CatalogService.Domain.Catalog;

public sealed class ProductImage
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ProductId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string AltText { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public bool IsPrimary { get; private set; }

    private ProductImage()
    {
    }

    public static ProductImage Create(Guid productId, string url, string altText, int sortOrder, bool isPrimary = false)
    {
        if (productId == Guid.Empty) throw new ArgumentException("Product id cannot be empty.", nameof(productId));
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        return new ProductImage
        {
            ProductId = productId,
            Url = url.Trim(),
            AltText = altText.Trim(),
            SortOrder = sortOrder,
            IsPrimary = isPrimary
        };
    }

    public void MarkPrimary()
    {
        IsPrimary = true;
    }

    public void UnmarkPrimary()
    {
        IsPrimary = false;
    }
}
