using CatalogService.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Brands;

public sealed record GetBrandsQuery : IRequest<IReadOnlyCollection<BrandDto>>;

public sealed class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, IReadOnlyCollection<BrandDto>>
{
    private readonly ICatalogDbContext _context;

    public GetBrandsQueryHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<BrandDto>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        => await _context.Brands
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new BrandDto(x.Id, x.Name, x.Slug, x.IsActive))
            .ToArrayAsync(cancellationToken);
}
