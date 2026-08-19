using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Brands;

public sealed record UpdateBrandCommand(Guid Id, string Name, string Slug, bool IsActive) : IRequest<Unit>;

public sealed class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
{
    public UpdateBrandCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(140).Matches("^[a-z0-9-]+$");
    }
}

public sealed class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Unit>
{
    private readonly ICatalogDbContext _context;

    public UpdateBrandCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Brand not found.");
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (await _context.Brands.AnyAsync(x => x.Id != request.Id && x.Slug == slug, cancellationToken))
        {
            throw new ConflictException("Brand slug already exists.");
        }

        brand.Update(request.Name, slug, request.IsActive);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
