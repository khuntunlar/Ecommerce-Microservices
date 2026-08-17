# Phase 02: Product
Service: Catalog Service

## Purpose
Build the product catalog boundary for the ecommerce platform. Catalog owns product browsing data, including products, categories, brands, and product images.

## Database Ownership
- Identity Service database: `identity_service`
- Catalog Service database: `catalog_service`

Each service owns its own database. Product data must not be stored in the Identity database.

## Completed Backend Slice
- Scaffolded Catalog Service with Clean Architecture style projects.
- Added Category, Brand, Product, and ProductImage domain entities.
- Added MySQL schema for `catalog_service`.
- Added CRUD APIs for categories, brands, and products.
- Added product image APIs for list, add, set primary, and delete.
- Added CQRS handlers with MediatR and FluentValidation.
- Added public read APIs and Admin-only write APIs with JWT role authorization.
- Added search, filters, sorting, and pagination for product browsing.
- Added OpenAPI JSON and Swagger UI for Catalog Service.
- Added Docker Compose services for `catalog-api` and `catalog-mysql`.
- Added unit, API, integration, frontend, and runtime smoke verification.

## Completed Frontend Slice
- Added public product listing page at `/products`.
- Added product detail page at `/products/{id}`.
- Added protected Admin Catalog workspace at `/admin/catalog`.
- Added server-side admin proxy routes that forward the user session JWT to Catalog Service.

## Acceptance Criteria
- Catalog Service builds independently
- Catalog Service tests pass
- `catalog_service` schema can be created separately from `identity_service`
- Product/category/brand APIs are available under `/api/v1`
- Product image APIs are available under `/api/v1/products/{productId}/images`
- Catalog write APIs require an Admin JWT role
- Catalog frontend pages build and pass tests
