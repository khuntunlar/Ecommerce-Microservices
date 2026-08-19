using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Brands;

public sealed record DeleteBrandCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, Unit>
{
    private readonly ICatalogDbContext _context;

    public DeleteBrandCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Brand not found.");

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
