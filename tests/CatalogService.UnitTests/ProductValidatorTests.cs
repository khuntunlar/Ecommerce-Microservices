using CatalogService.Application.Products;

namespace CatalogService.UnitTests;

public sealed class ProductValidatorTests
{
    [Fact]
    public void CreateProductValidator_RejectsNegativePrice()
    {
        var validator = new CreateProductCommandValidator();
        var result = validator.Validate(new CreateProductCommand(Guid.NewGuid(), Guid.NewGuid(), "Runner", "runner", "Shoe", -1, "RUNNER-1"));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateProductValidator_RejectsEmptyCategory()
    {
        var validator = new CreateProductCommandValidator();
        var result = validator.Validate(new CreateProductCommand(Guid.Empty, Guid.NewGuid(), "Runner", "runner", "Shoe", 10, "RUNNER-1"));

        Assert.False(result.IsValid);
    }
}
