using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Products;

public sealed record UpdateProductCommand(Guid Id, Guid CategoryId, Guid BrandId, string Name, string Slug, string Description, decimal Price, string Sku, bool IsActive) : IRequest<Unit>;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.BrandId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(180);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(220).Matches("^[a-z0-9-]+$");
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(80);
    }
}

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly ICatalogDbContext _context;

    public UpdateProductCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        await EnsureCategoryAndBrandExistAsync(request.CategoryId, request.BrandId, cancellationToken);
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Product not found.");
        var slug = request.Slug.Trim().ToLowerInvariant();
        var sku = request.Sku.Trim().ToUpperInvariant();

        if (await _context.Products.AnyAsync(x => x.Id != request.Id && x.Slug == slug, cancellationToken))
        {
            throw new ConflictException("Product slug already exists.");
        }

        if (await _context.Products.AnyAsync(x => x.Id != request.Id && x.Sku == sku, cancellationToken))
        {
            throw new ConflictException("Product SKU already exists.");
        }

        product.Update(request.CategoryId, request.BrandId, request.Name, slug, request.Description, request.Price, sku, request.IsActive);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
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
