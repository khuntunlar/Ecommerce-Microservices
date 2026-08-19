using CatalogService.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CatalogService.ApiTests;

public sealed class CatalogOpenApiTests
{
    [Fact]
    public void OpenApiDocumentIncludesCatalogEndpointsAndBearerSecurity()
    {
        var controller = new OpenApiController();
        var result = Assert.IsType<OkObjectResult>(controller.GetDocument());
        var json = JsonSerializer.Serialize(result.Value);

        Assert.Contains("/api/v1/products", json);
        Assert.Contains("/api/v1/products/{productId}/images", json);
        Assert.Contains("PagedProductResult", json);
        Assert.Contains("Bearer", json);
        Assert.Contains("Admin role required", json);
    }
}
