using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Products;

public sealed record DeleteProductCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly ICatalogDbContext _context;

    public DeleteProductCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Product not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
