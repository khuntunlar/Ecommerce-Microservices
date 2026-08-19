# Product Testing

## Current Tests
- Catalog unit tests for validators and application rules.
- Catalog API tests for health, authorization, and OpenAPI contract coverage.
- Catalog integration smoke test for database wiring.
- Frontend tests for auth session behavior, public catalog pages, and Admin Catalog UI structure.
- Runtime smoke tests documented in `doc/cli/cli.md`.

## Verified Commands
- `dotnet test CatalogService.slnx -c Release`
- `npm test`
- `npm run build`
- `docker compose up -d --build`
- Catalog API curl smoke tests for health, CRUD, search, product images, authorization, and OpenAPI.

## Recommended Next Tests
- Full browser end-to-end tests for customer product browsing.
- Full browser end-to-end tests for Admin Catalog create/update/delete flows.
- More conflict coverage for duplicate slugs and duplicate SKUs.
- More negative tests for missing category, missing brand, and invalid product image ownership.
