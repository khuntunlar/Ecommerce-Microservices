using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Categories;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICatalogDbContext _context;

    public DeleteCategoryCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Category not found.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
