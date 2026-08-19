using CatalogService.Application.ProductImages;

namespace CatalogService.UnitTests;

public sealed class ProductImageValidatorTests
{
    [Theory]
    [InlineData("")]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/image.jpg")]
    public void AddProductImageValidator_RejectsInvalidUrl(string url)
    {
        var validator = new AddProductImageCommandValidator();
        var result = validator.Validate(new AddProductImageCommand(Guid.NewGuid(), url, "Runner side view", 0, false));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void AddProductImageValidator_RejectsNegativeSortOrder()
    {
        var validator = new AddProductImageCommandValidator();
        var result = validator.Validate(new AddProductImageCommand(Guid.NewGuid(), "https://cdn.tun.shop/runner.jpg", "Runner side view", -1, false));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void AddProductImageValidator_AcceptsHttpsUrl()
    {
        var validator = new AddProductImageCommandValidator();
        var result = validator.Validate(new AddProductImageCommand(Guid.NewGuid(), "https://cdn.tun.shop/runner.jpg", "Runner side view", 1, true));

        Assert.True(result.IsValid);
    }
}
