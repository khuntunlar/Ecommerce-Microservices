using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.ProductImages;

public sealed record GetProductImagesQuery(Guid ProductId) : IRequest<IReadOnlyCollection<ProductImageDto>>;

public sealed class GetProductImagesQueryHandler : IRequestHandler<GetProductImagesQuery, IReadOnlyCollection<ProductImageDto>>
{
    private readonly ICatalogDbContext _context;

    public GetProductImagesQueryHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<ProductImageDto>> Handle(GetProductImagesQuery request, CancellationToken cancellationToken)
    {
        if (!await _context.Products.AnyAsync(x => x.Id == request.ProductId, cancellationToken))
        {
            throw new NotFoundException("Product not found.");
        }

        return await _context.ProductImages
            .AsNoTracking()
            .Where(x => x.ProductId == request.ProductId)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(x => new ProductImageDto(x.Id, x.ProductId, x.Url, x.AltText, x.SortOrder, x.IsPrimary))
            .ToArrayAsync(cancellationToken);
    }
}
