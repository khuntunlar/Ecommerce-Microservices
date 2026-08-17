using CatalogService.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Categories;

public sealed record GetCategoriesQuery : IRequest<IReadOnlyCollection<CategoryDto>>;

public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyCollection<CategoryDto>>
{
    private readonly ICatalogDbContext _context;

    public GetCategoriesQueryHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        => await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto(x.Id, x.Name, x.Slug, x.IsActive))
            .ToArrayAsync(cancellationToken);
}
