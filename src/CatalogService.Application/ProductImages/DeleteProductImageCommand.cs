using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.ProductImages;

public sealed record DeleteProductImageCommand(Guid ProductId, Guid ImageId) : IRequest<Unit>;

public sealed class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand, Unit>
{
    private readonly ICatalogDbContext _context;

    public DeleteProductImageCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _context.ProductImages
            .FirstOrDefaultAsync(x => x.Id == request.ImageId && x.ProductId == request.ProductId, cancellationToken)
            ?? throw new NotFoundException("Product image not found.");

        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
