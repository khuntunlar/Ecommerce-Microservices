using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Domain.Catalog;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.ProductImages;

public sealed record AddProductImageCommand(Guid ProductId, string Url, string AltText, int SortOrder, bool IsPrimary) : IRequest<ProductImageDto>;

public sealed class AddProductImageCommandValidator : AbstractValidator<AddProductImageCommand>
{
    public AddProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Url).NotEmpty().MaximumLength(500).Must(BeAbsoluteHttpUrl)
            .WithMessage("'Url' must be an absolute http or https URL.");
        RuleFor(x => x.AltText).NotEmpty().MaximumLength(180);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }

    private static bool BeAbsoluteHttpUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}

public sealed class AddProductImageCommandHandler : IRequestHandler<AddProductImageCommand, ProductImageDto>
{
    private readonly ICatalogDbContext _context;

    public AddProductImageCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ProductImageDto> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Products.AnyAsync(x => x.Id == request.ProductId, cancellationToken))
        {
            throw new NotFoundException("Product not found.");
        }

        if (request.IsPrimary)
        {
            await ClearPrimaryImagesAsync(request.ProductId, cancellationToken);
        }

        var image = ProductImage.Create(request.ProductId, request.Url, request.AltText, request.SortOrder, request.IsPrimary);
        _context.ProductImages.Add(image);
        await _context.SaveChangesAsync(cancellationToken);

        return new ProductImageDto(image.Id, image.ProductId, image.Url, image.AltText, image.SortOrder, image.IsPrimary);
    }

    private async Task ClearPrimaryImagesAsync(Guid productId, CancellationToken cancellationToken)
    {
        var images = await _context.ProductImages
            .Where(x => x.ProductId == productId && x.IsPrimary)
            .ToArrayAsync(cancellationToken);

        foreach (var image in images)
        {
            image.UnmarkPrimary();
        }
    }
}
