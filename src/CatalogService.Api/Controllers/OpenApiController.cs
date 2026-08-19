using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers;

[ApiController]
public sealed class OpenApiController : ControllerBase
{
    [HttpGet("openapi/v1.json")]
    public IActionResult GetDocument()
    {
        var document = new
        {
            openapi = "3.0.1",
            info = new
            {
                title = "Catalog Service API",
                version = "v1",
                description = "Phase-2 catalog API for categories, brands, products, and product images. Read endpoints are public; write endpoints require Admin role."
            },
            servers = new[] { new { url = "/" } },
            paths = new Dictionary<string, object>
            {
                ["/api/v1/categories"] = CollectionPath("Categories", "List categories", "Create category", "Category", "CategoryRequest"),
                ["/api/v1/categories/{id}"] = ResourcePath("Categories", "Update category", "Delete category", "CategoryRequest"),
                ["/api/v1/brands"] = CollectionPath("Brands", "List brands", "Create brand", "Brand", "BrandRequest"),
                ["/api/v1/brands/{id}"] = ResourcePath("Brands", "Update brand", "Delete brand", "BrandRequest"),
                ["/api/v1/products"] = ProductsCollectionPath(),
                ["/api/v1/products/{id}"] = ProductResourcePath(),
                ["/api/v1/products/{productId}/images"] = ProductImagesCollectionPath(),
                ["/api/v1/products/{productId}/images/{imageId}/primary"] = new
                {
                    put = Operation(
                        "Product Images",
                        "Set primary product image",
                        "Mark one product image as primary. Admin role required.",
                        null,
                        true,
                        204,
                        parameters: new[] { PathParameter("productId"), PathParameter("imageId") })
                },
                ["/api/v1/products/{productId}/images/{imageId}"] = new
                {
                    delete = Operation(
                        "Product Images",
                        "Delete product image",
                        "Delete a product image. Admin role required.",
                        null,
                        true,
                        204,
                        parameters: new[] { PathParameter("productId"), PathParameter("imageId") })
                },
                ["/api/v1/health"] = new
                {
                    get = new
                    {
                        tags = new[] { "Health" },
                        summary = "Health check",
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new { description = "Service is healthy" }
                        }
                    }
                }
            },
            components = new
            {
                securitySchemes = new Dictionary<string, object>
                {
                    ["Bearer"] = new
                    {
                        type = "http",
                        scheme = "bearer",
                        bearerFormat = "JWT"
                    }
                },
                schemas = Schemas()
            }
        };

        return Ok(document);
    }

    [HttpGet("swagger")]
    public ContentResult GetSwaggerUi()
    {
        const string html = """
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <title>Catalog Service API</title>
  <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css" />
</head>
<body>
  <div id="swagger-ui"></div>
  <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
  <script>SwaggerUIBundle({ url: '/openapi/v1.json', dom_id: '#swagger-ui' });</script>
</body>
</html>
""";

        return Content(html, "text/html");
    }

    private static object CollectionPath(string tag, string listSummary, string createSummary, string schema, string requestSchema) => new
    {
        get = Operation(tag, listSummary, $"Public endpoint to {listSummary.ToLowerInvariant()}.", $"{schema}List", false),
        post = Operation(tag, createSummary, $"Create a {schema.ToLowerInvariant()}. Admin role required.", schema, true, 201, bodySchema: requestSchema)
    };

    private static object ResourcePath(string tag, string updateSummary, string deleteSummary, string requestSchema) => new
    {
        put = Operation(tag, updateSummary, $"Update a resource. Admin role required.", null, true, 204, PathParameter("id"), bodySchema: requestSchema),
        delete = Operation(tag, deleteSummary, $"Delete a resource. Admin role required.", null, true, 204, PathParameter("id"))
    };

    private static object ProductsCollectionPath() => new
    {
        get = Operation(
            "Products",
            "Search products",
            "Public endpoint for product search, filtering, sorting, and pagination.",
            "PagedProductResult",
            false,
            parameters: ProductQueryParameters()),
        post = Operation(
            "Products",
            "Create product",
            "Create a product. Admin role required.",
            "Product",
            true,
            201,
            bodySchema: "ProductRequest")
    };

    private static object ProductResourcePath() => new
    {
        get = Operation("Products", "Get product by id", "Public endpoint to fetch one product.", "Product", false, parameters: new[] { PathParameter("id") }),
        put = Operation("Products", "Update product", "Update a product. Admin role required.", null, true, 204, PathParameter("id"), bodySchema: "ProductRequest"),
        delete = Operation("Products", "Delete product", "Delete a product. Admin role required.", null, true, 204, PathParameter("id"))
    };

    private static object ProductImagesCollectionPath() => new
    {
        get = Operation("Product Images", "List product images", "Public endpoint to list images for one product.", "ProductImageList", false, parameters: new[] { PathParameter("productId") }),
        post = Operation("Product Images", "Add product image", "Add an image to one product. Admin role required.", "ProductImage", true, 201, PathParameter("productId"), bodySchema: "ProductImageRequest")
    };

    private static Dictionary<string, object> Operation(
        string tag,
        string summary,
        string description,
        string? successSchema,
        bool authorize,
        int successStatus = 200,
        object? parameter = null,
        IEnumerable<object>? parameters = null,
        string? bodySchema = null)
    {
        var allParameters = new List<object>();
        if (parameter is not null)
        {
            allParameters.Add(parameter);
        }

        if (parameters is not null)
        {
            allParameters.AddRange(parameters);
        }

        var operation = new Dictionary<string, object>
        {
            ["tags"] = new[] { tag },
            ["summary"] = summary,
            ["description"] = description,
            ["responses"] = Responses(successSchema, successStatus)
        };

        if (allParameters.Count > 0)
        {
            operation["parameters"] = allParameters;
        }

        if (bodySchema is not null)
        {
            operation["requestBody"] = RequestBody(bodySchema);
        }

        if (authorize)
        {
            operation["security"] = BearerSecurity();
        }

        return operation;
    }

    private static object PathParameter(string name) => new
    {
        name,
        @in = "path",
        required = true,
        schema = new { type = "string", format = "uuid" }
    };

    private static object QueryParameter(string name, string type, string? format = null)
    {
        var schema = new Dictionary<string, object> { ["type"] = type };
        if (format is not null)
        {
            schema["format"] = format;
        }

        return new
        {
            name,
            @in = "query",
            required = false,
            schema
        };
    }

    private static object[] ProductQueryParameters() => new[]
    {
        QueryParameter("search", "string"),
        QueryParameter("categoryId", "string", "uuid"),
        QueryParameter("brandId", "string", "uuid"),
        QueryParameter("isActive", "boolean"),
        QueryParameter("minPrice", "number", "decimal"),
        QueryParameter("maxPrice", "number", "decimal"),
        QueryParameter("sortBy", "string"),
        QueryParameter("sortDirection", "string"),
        QueryParameter("page", "integer", "int32"),
        QueryParameter("pageSize", "integer", "int32")
    };

    private static object RequestBody(string schema) => new
    {
        required = true,
        content = new Dictionary<string, object>
        {
            ["application/json"] = new
            {
                schema = new Dictionary<string, object> { ["$ref"] = $"#/components/schemas/{schema}" }
            }
        }
    };

    private static Dictionary<string, object>[] BearerSecurity() =>
        new[] { new Dictionary<string, object> { ["Bearer"] = Array.Empty<string>() } };

    private static Dictionary<string, object> Responses(string? successSchema, int successStatus = 200)
    {
        var responses = new Dictionary<string, object>
        {
            [successStatus.ToString()] = successSchema is null
                ? new { description = "Success" }
                : new
                {
                    description = "Success",
                    content = new Dictionary<string, object>
                    {
                        ["application/json"] = new
                        {
                            schema = new Dictionary<string, object> { ["$ref"] = $"#/components/schemas/{successSchema}" }
                        }
                    }
                },
            ["400"] = Problem("Validation failed"),
            ["401"] = Problem("Authentication failed"),
            ["403"] = Problem("Forbidden"),
            ["404"] = Problem("Not found"),
            ["409"] = Problem("Conflict"),
            ["500"] = Problem("Unexpected error")
        };

        return responses;
    }

    private static object Problem(string description) => new
    {
        description,
        content = new Dictionary<string, object>
        {
            ["application/problem+json"] = new
            {
                schema = new Dictionary<string, object> { ["$ref"] = "#/components/schemas/ProblemDetails" }
            }
        }
    };

    private static Dictionary<string, object> Schemas() => new()
    {
        ["CategoryRequest"] = new { type = "object", required = new[] { "name", "slug" }, properties = BasicCatalogRequestProperties() },
        ["BrandRequest"] = new { type = "object", required = new[] { "name", "slug" }, properties = BasicCatalogRequestProperties() },
        ["Category"] = BasicCatalogSchema(),
        ["Brand"] = BasicCatalogSchema(),
        ["CategoryList"] = ArrayOf("Category"),
        ["BrandList"] = ArrayOf("Brand"),
        ["ProductRequest"] = new
        {
            type = "object",
            required = new[] { "categoryId", "brandId", "name", "slug", "description", "price", "sku" },
            properties = ProductRequestProperties()
        },
        ["Product"] = new { type = "object", properties = ProductProperties() },
        ["PagedProductResult"] = new
        {
            type = "object",
            properties = new Dictionary<string, object>
            {
                ["items"] = ArrayOf("Product"),
                ["page"] = new { type = "integer", format = "int32" },
                ["pageSize"] = new { type = "integer", format = "int32" },
                ["totalItems"] = new { type = "integer", format = "int32" },
                ["totalPages"] = new { type = "integer", format = "int32" },
                ["hasPreviousPage"] = new { type = "boolean" },
                ["hasNextPage"] = new { type = "boolean" }
            }
        },
        ["ProductImageRequest"] = new
        {
            type = "object",
            required = new[] { "url", "altText" },
            properties = new Dictionary<string, object>
            {
                ["url"] = new { type = "string", format = "uri" },
                ["altText"] = new { type = "string" },
                ["sortOrder"] = new { type = "integer", format = "int32" },
                ["isPrimary"] = new { type = "boolean" }
            }
        },
        ["ProductImage"] = new
        {
            type = "object",
            properties = new Dictionary<string, object>
            {
                ["id"] = new { type = "string", format = "uuid" },
                ["productId"] = new { type = "string", format = "uuid" },
                ["url"] = new { type = "string", format = "uri" },
                ["altText"] = new { type = "string" },
                ["sortOrder"] = new { type = "integer", format = "int32" },
                ["isPrimary"] = new { type = "boolean" }
            }
        },
        ["ProductImageList"] = ArrayOf("ProductImage"),
        ["ProblemDetails"] = new
        {
            type = "object",
            properties = new Dictionary<string, object>
            {
                ["title"] = new { type = "string" },
                ["status"] = new { type = "integer", format = "int32" },
                ["detail"] = new { type = "string" }
            }
        }
    };

    private static object BasicCatalogSchema() => new
    {
        type = "object",
        properties = new Dictionary<string, object>
        {
            ["id"] = new { type = "string", format = "uuid" },
            ["name"] = new { type = "string" },
            ["slug"] = new { type = "string" },
            ["isActive"] = new { type = "boolean" }
        }
    };

    private static Dictionary<string, object> BasicCatalogRequestProperties() => new()
    {
        ["name"] = new { type = "string" },
        ["slug"] = new { type = "string" },
        ["isActive"] = new { type = "boolean" }
    };

    private static Dictionary<string, object> ProductRequestProperties() => new()
    {
        ["categoryId"] = new { type = "string", format = "uuid" },
        ["brandId"] = new { type = "string", format = "uuid" },
        ["name"] = new { type = "string" },
        ["slug"] = new { type = "string" },
        ["description"] = new { type = "string" },
        ["price"] = new { type = "number", format = "decimal" },
        ["sku"] = new { type = "string" },
        ["isActive"] = new { type = "boolean" }
    };

    private static Dictionary<string, object> ProductProperties()
    {
        var properties = ProductRequestProperties();
        properties.Insert("id", new { type = "string", format = "uuid" });
        return properties;
    }

    private static object ArrayOf(string schema) => new
    {
        type = "array",
        items = new Dictionary<string, object> { ["$ref"] = $"#/components/schemas/{schema}" }
    };
}

internal static class DictionaryExtensions
{
    public static void Insert(this Dictionary<string, object> dictionary, string key, object value)
    {
        dictionary[key] = value;
    }
}
