using CatalogService.Application.Abstractions;

namespace CatalogService.Infrastructure.Persistence;

public sealed class CatalogDbContextFactory
{
    private readonly CatalogDbContext _context;

    public CatalogDbContextFactory(CatalogDbContext context)
    {
        _context = context;
    }

    public ICatalogDbContext Create() => _context;
}
