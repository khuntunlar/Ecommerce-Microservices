using CatalogService.Application.Products;

namespace CatalogService.UnitTests;

public sealed class ProductSearchValidatorTests
{
    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public void GetProductsValidator_RejectsInvalidPaging(int page, int pageSize)
    {
        var validator = new GetProductsQueryValidator();
        var result = validator.Validate(NewQuery(page: page, pageSize: pageSize));

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("unknown", "asc")]
    [InlineData("name", "sideways")]
    public void GetProductsValidator_RejectsInvalidSorting(string sortBy, string sortDirection)
    {
        var validator = new GetProductsQueryValidator();
        var result = validator.Validate(NewQuery(sortBy: sortBy, sortDirection: sortDirection));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void GetProductsValidator_RejectsMaxPriceBelowMinPrice()
    {
        var validator = new GetProductsQueryValidator();
        var result = validator.Validate(NewQuery(minPrice: 50, maxPrice: 10));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void GetProductsValidator_AcceptsValidSearch()
    {
        var validator = new GetProductsQueryValidator();
        var result = validator.Validate(NewQuery(sortBy: "price", sortDirection: "desc", page: 2, pageSize: 10));

        Assert.True(result.IsValid);
    }

    private static GetProductsQuery NewQuery(
        string? sortBy = null,
        string? sortDirection = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int page = 1,
        int pageSize = 20)
        => new(null, null, null, null, minPrice, maxPrice, sortBy, sortDirection, page, pageSize);
}
