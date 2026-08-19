using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Products;

public sealed record GetProductsQuery(
    string? Search,
    Guid? CategoryId,
    Guid? BrandId,
    bool? IsActive,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? SortBy,
    string? SortDirection,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<ProductDto>>;

public sealed class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    private static readonly string[] AllowedSortFields = ["name", "price", "createdAt", "sku"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
        RuleFor(x => x).Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MaxPrice >= x.MinPrice)
            .WithMessage("MaxPrice must be greater than or equal to MinPrice.");
        RuleFor(x => x.SortBy).Must(sortBy => IsAllowed(sortBy, AllowedSortFields))
            .WithMessage("SortBy must be one of: name, price, createdAt, sku.");
        RuleFor(x => x.SortDirection).Must(direction => IsAllowed(direction, AllowedSortDirections))
            .WithMessage("SortDirection must be one of: asc, desc.");
    }

    private static bool IsAllowed(string? value, IReadOnlyCollection<string> allowedValues)
    {
        return string.IsNullOrWhiteSpace(value)
            || allowedValues.Contains(value.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly ICatalogDbContext _context;

    public GetProductsQueryHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.Name.Contains(search) || x.Sku.Contains(search) || x.Description.Contains(search));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);
        }

        if (request.BrandId.HasValue)
        {
            query = query.Where(x => x.BrandId == request.BrandId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(x => x.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(x => x.Price <= request.MaxPrice.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var page = request.Page;
        var pageSize = request.PageSize;

        var items = await query
            .ApplySorting(request.SortBy, request.SortDirection)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ProductDto(x.Id, x.CategoryId, x.BrandId, x.Name, x.Slug, x.Description, x.Price, x.Sku, x.IsActive))
            .ToArrayAsync(cancellationToken);

        return new PagedResult<ProductDto>(items, page, pageSize, totalItems);
    }
}

internal static class ProductQuerySortingExtensions
{
    public static IQueryable<Domain.Catalog.Product> ApplySorting(
        this IQueryable<Domain.Catalog.Product> query,
        string? sortBy,
        string? sortDirection)
    {
        var descending = string.Equals(sortDirection?.Trim(), "desc", StringComparison.OrdinalIgnoreCase);
        var normalizedSortBy = string.IsNullOrWhiteSpace(sortBy) ? "name" : sortBy.Trim();

        return normalizedSortBy.ToLowerInvariant() switch
        {
            "price" => descending ? query.OrderByDescending(x => x.Price).ThenBy(x => x.Name) : query.OrderBy(x => x.Price).ThenBy(x => x.Name),
            "createdat" => descending ? query.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Name) : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Name),
            "sku" => descending ? query.OrderByDescending(x => x.Sku).ThenBy(x => x.Name) : query.OrderBy(x => x.Sku).ThenBy(x => x.Name),
            _ => descending ? query.OrderByDescending(x => x.Name).ThenBy(x => x.Id) : query.OrderBy(x => x.Name).ThenBy(x => x.Id)
        };
    }
}
