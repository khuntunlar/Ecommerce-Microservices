# Product Requirements

## Business Goals
- Allow customers to browse products by category and brand.
- Keep catalog data separate from identity, inventory, cart, and order data.
- Provide product data for later inventory, cart, checkout, and order phases.

## Functional Requirements
- Create, update, delete, and list categories.
- Create, update, delete, and list brands.
- Create, update, delete, list, and fetch products.
- Products must belong to a category and brand.
- Products must have unique slugs and SKUs.
- Product images are part of the catalog model.

## Non-Functional Requirements
- Catalog has its own database: `catalog_service`.
- Catalog API returns consistent ProblemDetails errors.
- Catalog APIs are versioned under `/api/v1`.
- Product data should be queryable without contacting Identity Service.
