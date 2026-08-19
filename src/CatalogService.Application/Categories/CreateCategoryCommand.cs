using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Domain.Catalog;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Categories;

public sealed record CreateCategoryCommand(string Name, string Slug) : IRequest<CategoryDto>;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(140).Matches("^[a-z0-9-]+$");
    }
}

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICatalogDbContext _context;

    public CreateCategoryCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();
        if (await _context.Categories.AnyAsync(x => x.Slug == slug, cancellationToken))
        {
            throw new ConflictException("Category slug already exists.");
        }

        var category = Category.Create(request.Name, slug);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        return new CategoryDto(category.Id, category.Name, category.Slug, category.IsActive);
    }
}
