using CatalogService.Application.Categories;

namespace CatalogService.UnitTests;

public sealed class CategoryValidatorTests
{
    [Theory]
    [InlineData("")]
    [InlineData("Bad Slug")]
    [InlineData("bad_slug")]
    public void CreateCategoryValidator_RejectsInvalidSlug(string slug)
    {
        var validator = new CreateCategoryCommandValidator();
        var result = validator.Validate(new CreateCategoryCommand("Shoes", slug));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateCategoryValidator_AcceptsKebabSlug()
    {
        var validator = new CreateCategoryCommandValidator();
        var result = validator.Validate(new CreateCategoryCommand("Shoes", "running-shoes"));

        Assert.True(result.IsValid);
    }
}
