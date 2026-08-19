using System.Reflection;
using CatalogService.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.ApiTests;

public sealed class CatalogWriteAuthorizationTests
{
    private static readonly Type[] CatalogControllers =
    [
        typeof(CategoriesController),
        typeof(BrandsController),
        typeof(ProductsController),
        typeof(ProductImagesController)
    ];

    [Fact]
    public void WriteEndpointsRequireAdminRole()
    {
        foreach (var controller in CatalogControllers)
        {
            var writeActions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method =>
                    method.GetCustomAttribute<HttpPostAttribute>() is not null
                    || method.GetCustomAttribute<HttpPutAttribute>() is not null
                    || method.GetCustomAttribute<HttpDeleteAttribute>() is not null);

            foreach (var action in writeActions)
            {
                var authorize = action.GetCustomAttribute<AuthorizeAttribute>();

                Assert.NotNull(authorize);
                Assert.Equal("Admin", authorize.Roles);
            }
        }
    }

    [Fact]
    public void ReadEndpointsStayPublic()
    {
        foreach (var controller in CatalogControllers)
        {
            var readActions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.GetCustomAttribute<HttpGetAttribute>() is not null);

            foreach (var action in readActions)
            {
                Assert.Null(action.GetCustomAttribute<AuthorizeAttribute>());
            }
        }
    }
}
