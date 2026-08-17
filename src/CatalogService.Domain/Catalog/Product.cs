namespace CatalogService.Domain.Catalog;

public sealed class Product
{
    private readonly List<ProductImage> _images = [];

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CategoryId { get; private set; }
    public Guid BrandId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    private Product()
    {
    }

    public static Product Create(Guid categoryId, Guid brandId, string name, string slug, string description, decimal price, string sku)
    {
        Validate(categoryId, brandId, name, slug, price, sku);

        return new Product
        {
            CategoryId = categoryId,
            BrandId = brandId,
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            Description = description.Trim(),
            Price = price,
            Sku = sku.Trim().ToUpperInvariant()
        };
    }

    public void Update(Guid categoryId, Guid brandId, string name, string slug, string description, decimal price, string sku, bool isActive)
    {
        Validate(categoryId, brandId, name, slug, price, sku);

        CategoryId = categoryId;
        BrandId = brandId;
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Description = description.Trim();
        Price = price;
        Sku = sku.Trim().ToUpperInvariant();
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddImage(string url, string altText, int sortOrder, bool isPrimary = false)
    {
        _images.Add(ProductImage.Create(Id, url, altText, sortOrder, isPrimary));
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(Guid categoryId, Guid brandId, string name, string slug, decimal price, string sku)
    {
        if (categoryId == Guid.Empty) throw new ArgumentException("Category id cannot be empty.", nameof(categoryId));
        if (brandId == Guid.Empty) throw new ArgumentException("Brand id cannot be empty.", nameof(brandId));
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
    }
}
