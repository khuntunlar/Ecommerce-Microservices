using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.ProductImages;

public sealed record SetPrimaryProductImageCommand(Guid ProductId, Guid ImageId) : IRequest<Unit>;

public sealed class SetPrimaryProductImageCommandHandler : IRequestHandler<SetPrimaryProductImageCommand, Unit>
{
    private readonly ICatalogDbContext _context;

    public SetPrimaryProductImageCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(SetPrimaryProductImageCommand request, CancellationToken cancellationToken)
    {
        var images = await _context.ProductImages
            .Where(x => x.ProductId == request.ProductId)
            .ToArrayAsync(cancellationToken);

        if (images.Length == 0)
        {
            throw new NotFoundException("Product image not found.");
        }

        var selected = images.FirstOrDefault(x => x.Id == request.ImageId)
            ?? throw new NotFoundException("Product image not found.");

        foreach (var image in images)
        {
            image.UnmarkPrimary();
        }

        selected.MarkPrimary();
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
