namespace CatalogService.Domain.Catalog;

public sealed class Brand
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    private Brand()
    {
    }

    public static Brand Create(string name, string slug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        return new Brand
        {
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant()
        };
    }

    public void Update(string name, string slug, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
