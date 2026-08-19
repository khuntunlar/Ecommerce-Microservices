using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Categories;

public sealed record UpdateCategoryCommand(Guid Id, string Name, string Slug, bool IsActive) : IRequest<Unit>;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(140).Matches("^[a-z0-9-]+$");
    }
}

public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Unit>
{
    private readonly ICatalogDbContext _context;

    public UpdateCategoryCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Category not found.");
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (await _context.Categories.AnyAsync(x => x.Id != request.Id && x.Slug == slug, cancellationToken))
        {
            throw new ConflictException("Category slug already exists.");
        }

        category.Update(request.Name, slug, request.IsActive);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
