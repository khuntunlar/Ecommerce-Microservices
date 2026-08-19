using CatalogService.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Abstractions;

public interface ICatalogDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Brand> Brands { get; }
    DbSet<ProductImage> ProductImages { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
