# Product API

Base paths:
- `/api/v1/categories`
- `/api/v1/brands`
- `/api/v1/products`
- `/api/v1/products/{productId}/images`
- `/openapi/v1.json`
- `/swagger`

## Categories
- `GET /api/v1/categories` - public
- `POST /api/v1/categories` - Admin role required
- `PUT /api/v1/categories/{id}` - Admin role required
- `DELETE /api/v1/categories/{id}` - Admin role required

## Brands
- `GET /api/v1/brands` - public
- `POST /api/v1/brands` - Admin role required
- `PUT /api/v1/brands/{id}` - Admin role required
- `DELETE /api/v1/brands/{id}` - Admin role required

## Products
- `GET /api/v1/products` - public
- `GET /api/v1/products/{id}` - public
- `POST /api/v1/products` - Admin role required
- `PUT /api/v1/products/{id}` - Admin role required
- `DELETE /api/v1/products/{id}` - Admin role required

## Product Images
- `GET /api/v1/products/{productId}/images` - public
- `POST /api/v1/products/{productId}/images` - Admin role required
- `PUT /api/v1/products/{productId}/images/{imageId}/primary` - Admin role required
- `DELETE /api/v1/products/{productId}/images/{imageId}` - Admin role required

## OpenAPI
- `GET /openapi/v1.json` - public OpenAPI contract
- `GET /swagger` - public Swagger UI

## Product Query Parameters
- `search`
- `categoryId`
- `brandId`
- `isActive`
- `minPrice`
- `maxPrice`
- `sortBy`: `name`, `price`, `createdAt`, `sku`
- `sortDirection`: `asc`, `desc`
- `page`: default `1`
- `pageSize`: default `20`, max `100`

## Product List Response
`GET /api/v1/products` returns a paged result:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "totalItems": 0,
  "totalPages": 0,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

## Error Shape
Validation and unexpected errors use ProblemDetails-style responses with HTTP status codes such as `400`, `401`, `403`, `404`, and `409`.
