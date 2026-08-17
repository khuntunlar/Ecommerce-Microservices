# Product Backend

## Architecture
- Catalog Service uses separate Api, Application, Domain, and Infrastructure projects.
- EF Core + Pomelo MySQL provider persists catalog data.
- Catalog owns product, category, brand, and product image entities.
- Controllers are thin HTTP adapters over CQRS commands and queries.
- MediatR dispatches Catalog use cases from the API layer to the Application layer.
- FluentValidation runs through an application pipeline behavior.
- Public read endpoints do not require Identity Service calls.
- Write endpoints require a valid JWT with the `Admin` role.
- API middleware returns consistent ProblemDetails-style errors.
- Catalog exposes manual OpenAPI JSON and Swagger UI endpoints.

## Projects
- `src/CatalogService.Api`
- `src/CatalogService.Application`
- `src/CatalogService.Domain`
- `src/CatalogService.Infrastructure`
- `tests/CatalogService.UnitTests`
- `tests/CatalogService.IntegrationTests`
- `tests/CatalogService.ApiTests`

## Implemented Use Cases
- Category create, update, delete, and list.
- Brand create, update, delete, and list.
- Product create, update, delete, list, and fetch by id.
- Product image list, add, set primary, and delete.
- Product search, category filter, brand filter, active filter, price range filter, sorting, and pagination.
