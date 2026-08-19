using CatalogService.Domain.Catalog;

namespace CatalogService.UnitTests;

public sealed class ProductTests
{
    [Fact]
    public void Create_NormalizesSlugAndSku()
    {
        var product = Product.Create(Guid.NewGuid(), Guid.NewGuid(), "  Running Shoe  ", "RUNNING-SHOE", "Daily shoe", 120, " sku-1 ");

        Assert.Equal("Running Shoe", product.Name);
        Assert.Equal("running-shoe", product.Slug);
        Assert.Equal("SKU-1", product.Sku);
    }

    [Fact]
    public void Create_RejectsNegativePrice()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Product.Create(Guid.NewGuid(), Guid.NewGuid(), "Product", "product", "Description", -1, "SKU"));
    }
}
