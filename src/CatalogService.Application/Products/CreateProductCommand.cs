using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Domain.Catalog;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Products;

public sealed record CreateProductCommand(Guid CategoryId, Guid BrandId, string Name, string Slug, string Description, decimal Price, string Sku) : IRequest<ProductDto>;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.BrandId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(180);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(220).Matches("^[a-z0-9-]+$");
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(80);
    }
}

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly ICatalogDbContext _context;

    public CreateProductCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        await EnsureCategoryAndBrandExistAsync(request.CategoryId, request.BrandId, cancellationToken);
        var slug = request.Slug.Trim().ToLowerInvariant();
        var sku = request.Sku.Trim().ToUpperInvariant();

        if (await _context.Products.AnyAsync(x => x.Slug == slug, cancellationToken))
        {
            throw new ConflictException("Product slug already exists.");
        }

        if (await _context.Products.AnyAsync(x => x.Sku == sku, cancellationToken))
        {
            throw new ConflictException("Product SKU already exists.");
        }

        var product = Product.Create(request.CategoryId, request.BrandId, request.Name, slug, request.Description, request.Price, sku);
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return new ProductDto(product.Id, product.CategoryId, product.BrandId, product.Name, product.Slug, product.Description, product.Price, product.Sku, product.IsActive);
    }

    private async Task EnsureCategoryAndBrandExistAsync(Guid categoryId, Guid brandId, CancellationToken cancellationToken)
    {
        if (!await _context.Categories.AnyAsync(x => x.Id == categoryId, cancellationToken))
        {
            throw new NotFoundException("Category not found.");
        }

        if (!await _context.Brands.AnyAsync(x => x.Id == brandId, cancellationToken))
        {
            throw new NotFoundException("Brand not found.");
        }
    }
}
