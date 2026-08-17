using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Domain.Catalog;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Brands;

public sealed record CreateBrandCommand(string Name, string Slug) : IRequest<BrandDto>;

public sealed class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(140).Matches("^[a-z0-9-]+$");
    }
}

public sealed class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, BrandDto>
{
    private readonly ICatalogDbContext _context;

    public CreateBrandCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<BrandDto> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();
        if (await _context.Brands.AnyAsync(x => x.Slug == slug, cancellationToken))
        {
            throw new ConflictException("Brand slug already exists.");
        }

        var brand = Brand.Create(request.Name, slug);
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync(cancellationToken);
        return new BrandDto(brand.Id, brand.Name, brand.Slug, brand.IsActive);
    }
}
