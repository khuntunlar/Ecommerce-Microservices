using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Products;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly ICatalogDbContext _context;

    public GetProductByIdQueryHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Product not found.");

        return new ProductDto(product.Id, product.CategoryId, product.BrandId, product.Name, product.Slug, product.Description, product.Price, product.Sku, product.IsActive);
    }
}
