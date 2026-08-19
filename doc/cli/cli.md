# CLI Command Log

This file records the command line work used to create and verify Phase 01 Authentication backend.

Secrets are redacted in this log. Replace `<mysql-password>` with the local MySQL password when rerunning MySQL commands.

## Environment Checks

```bash
dotnet --version
```

```bash
dotnet ef --version
```

Result: `dotnet-ef` was not installed in this environment.

```bash
which mysql
```

```bash
which docker
```

```bash
docker ps --format '{{.Names}} {{.Image}} {{.Ports}}'
```

Result: Docker daemon was not reachable from this environment.

## Project Scaffold

```bash
mkdir -p src tests
```

```bash
dotnet new sln -n IdentityService --force
```

```bash
dotnet new webapi -n IdentityService.Api -o src/IdentityService.Api --no-restore
```

```bash
dotnet new classlib -n IdentityService.Application -o src/IdentityService.Application --no-restore
```

```bash
dotnet new classlib -n IdentityService.Domain -o src/IdentityService.Domain --no-restore
```

```bash
dotnet new classlib -n IdentityService.Infrastructure -o src/IdentityService.Infrastructure --no-restore
```

```bash
dotnet new xunit -n IdentityService.UnitTests -o tests/IdentityService.UnitTests --no-restore
```

```bash
dotnet new xunit -n IdentityService.IntegrationTests -o tests/IdentityService.IntegrationTests --no-restore
```

```bash
dotnet new xunit -n IdentityService.ApiTests -o tests/IdentityService.ApiTests --no-restore
```

```bash
dotnet sln IdentityService.slnx add src/IdentityService.Api/IdentityService.Api.csproj src/IdentityService.Application/IdentityService.Application.csproj src/IdentityService.Domain/IdentityService.Domain.csproj src/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj tests/IdentityService.UnitTests/IdentityService.UnitTests.csproj tests/IdentityService.IntegrationTests/IdentityService.IntegrationTests.csproj tests/IdentityService.ApiTests/IdentityService.ApiTests.csproj
```

## Build And Test

```bash
dotnet build IdentityService.slnx -c Release
```

```bash
dotnet test IdentityService.slnx -c Release --no-build
```

Latest result:
- Build passed with `0 Warning(s), 0 Error(s)`
- Tests passed: API `1`, Unit `5`, Integration `3`

## MySQL Verification

Initial credential checks:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -uroot -p<mysql-password> -e "SELECT VERSION();"
```

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -uroot -e "SELECT VERSION();"
```

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -e "SELECT VERSION();"
```

Verified connection with provided credentials:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p<mysql-password> 
ecommerce_with_dot_net -e "SELECT DATABASE() AS db, VERSION() AS version; SHOW TABLES;"
```

Applied the Identity schema SQL migration:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p<mysql-password> ecommerce_with_dot_net --execute="source /home/khuntunlar/Documents/Ecommerce-Microservices/src/IdentityService.Infrastructure/Persistence/Migrations/202607030001_InitialIdentitySchema.mysql.sql"

dotnet ef database update \
  --project src/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj \
  --startup-project src/IdentityService.Api/IdentityService.Api.csproj
```

Verified tables, seeded role, and migration history:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p<mysql-password> ecommerce_with_dot_net -e "SHOW TABLES; SELECT Id, Name, NormalizedName FROM Roles; SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory;"


Note AboutPort Conflict:
mysql --protocol=TCP -h 127.0.0.1 -P 3307 -ukhuntunlar -p ecommerce_with_dot_net


= To see DB
docker compose -f docker-compose.yml -f docker-compose.local-mysql.yml run --rm --no-deps \
  local-mysql-init \
  mysql -h host.docker.internal -P 3306 -u"$MYSQL_USER" "$IDENTITY_MYSQL_DATABASE"
  

= Show Table
docker compose exec mysql mysql -ukhuntunlar -pkhuntunlar2024 ecommerce_with_dot_net \
  -e "SELECT * FROM Users;"
  
  = To confirm which database the API is using:
  docker compose exec identity-api printenv ConnectionStrings__Identity
  
  = Note CLI
  docker compose config
  docker compose up -d identity-mysql catalog-mysql
  docker compose ps
  docker compose exec identity-mysql mysql -u khuntunlar -p identity_service
  docker compose exec catalog-mysql mysql -u khuntunlar -p catalog_service
  docker compose -f docker-compose.yml -f docker-compose.local-mysql.yml exec identity-api printenv ConnectionStrings__Identity
  
  = Check Local TCP
  mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p ecommerce_with_dot_net
```

Latest MySQL result:
- Database: `ecommerce_with_dot_net`
- MySQL version: `5.7.43-log`
- Tables created: `AuditLogs`, `RefreshTokens`, `Roles`, `UserRoles`, `Users`, `__EFMigrationsHistory`
- Seeded role: `Customer`
- Migration history row: `202607030001_InitialIdentitySchema`

## Notes

- No `dotnet` installation command was run. The SDK was already available.
- No `dotnet-ef` installation command was run. `dotnet ef --version` confirmed the tool was missing.
- Migration verification was completed through the MySQL client using `202607030001_InitialIdentitySchema.mysql.sql`.

## Phase 01 Frontend And Docker

```bash
mkdir -p src/Ecommerce.Web/app/{account,forgot-password,login,logout,register,reset-password} src/Ecommerce.Web/components src/Ecommerce.Web/lib src/Ecommerce.Web/public
```

```bash
cd src/Ecommerce.Web
npm install
```

```bash
cd src/Ecommerce.Web
npm run build
```

Latest frontend result:
- Next.js production build passed.
- Routes generated: `/`, `/account`, `/forgot-password`, `/login`, `/logout`, `/register`, `/reset-password`.
- npm reported `2 moderate severity vulnerabilities`; no forced audit fix was applied.

```bash
dotnet build IdentityService.slnx -c Release
```

```bash
dotnet test IdentityService.slnx -c Release --no-build
```

Latest backend result:
- Build passed with `0 Warning(s), 0 Error(s)`.
- Tests passed: API `1`, Unit `5`, Integration `3`.

```bash
docker compose config --quiet
```

Latest Docker config result:
- Compose configuration is valid.

```bash
docker compose build
```

Latest Docker build result:
- Docker daemon was not reachable: `Cannot connect to the Docker daemon at unix:///home/khuntunlar/.docker/desktop/docker.sock`.

```bash
docker compose up --build
```

Use this after Docker Desktop or the Docker daemon is running.

## Docker MySQL Port Conflict Fix

Docker failed when publishing MySQL on host port `3306` because another local process already used that port.

Updated Compose mapping:

```yaml
ports:
  - "3307:3306"
```

Host machine MySQL access after this change:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3307 -ukhuntunlar -p<mysql-password> ecommerce_with_dot_net
```

Container-to-container access remains unchanged:

```text
Server=mysql;Port=3306;Database=ecommerce_with_dot_net;User=khuntunlar;Password=<mysql-password>;
```

Recreate the stack after the change:

```bash
docker compose down
```

```bash
docker compose up --build
```

## EF Backing Field Conflict Fix

Register/login failed with:

```text
The member 'User._refreshTokens' cannot use field '_refreshTokens' because it is already used by 'User.RefreshTokens'.
```

Verification after mapping `User.RefreshTokens` and `User.Roles` through their backing fields:

```bash
dotnet test IdentityService.slnx -c Release
```

Latest result:
- API tests: `1` passed.
- Unit tests: `5` passed.
- Integration tests: `3` passed.

## Register 500 AuditLog CreatedAt Fix

Register failed with generic `500 Unexpected error`. Docker logs showed the real database error:

```text
MySqlException: Field 'CreatedAt' doesn't have a default value
```

Fixes applied:
- Added `CreatedAt` to `AuditLog` so EF inserts the required database column.
- Registered MediatR validation behavior and authentication validators in the API dependency container.

Verification:

```bash
dotnet test IdentityService.slnx -c Release
```

Latest result:
- API tests: `1` passed.
- Unit tests: `5` passed.
- Integration tests: `3` passed.

Rebuilt and restarted Docker stack:

```bash
docker compose up -d --build
```

Live register/login verification against Dockerized API:

```bash
curl -i -X POST http://localhost:5294/api/v1/auth/register \
  -H 'Content-Type: application/json' \
  -d '{"displayName":"Codex Test","email":"codex-test-<timestamp>@tun.shop","password":"admin123"}'
```

Result: `201 Created`.

```bash
curl -i -X POST http://localhost:5294/api/v1/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"codex-test-<timestamp>@tun.shop","password":"admin123"}'
```

Result: `200 OK`.

## Compose MySQL Modes For Learning

Added support for both Docker MySQL and local machine MySQL.

Files:
- `docker-compose.yml`: default Docker MySQL mode.
- `docker-compose.local-mysql.yml`: override for local machine MySQL/phpMyAdmin storage.
- `docker-compose.mysql-learning.yml`: commented examples only, not for direct running.

Validate default Docker MySQL mode:

```bash
docker compose config --quiet
```

Run default Docker MySQL mode:

```bash
docker compose up --build
```

Validate local MySQL override mode:

```bash
docker compose -f docker-compose.yml -f docker-compose.local-mysql.yml config --quiet
```

Run local MySQL override mode:

```bash
docker compose -f docker-compose.yml -f docker-compose.local-mysql.yml up --build
```

Local MySQL mode changes the API connection string to:

```text
Server=host.docker.internal;Port=3306;Database=ecommerce_with_dot_net;User=khuntunlar;Password=<mysql-password>;
```

Docker MySQL mode keeps the API connection string as:

```text
Server=mysql;Port=3306;Database=ecommerce_with_dot_net;User=khuntunlar;Password=<mysql-password>;
```

## Phase 01 Hardening Completion

Implemented remaining Phase-1 hardening items:
- OpenAPI JSON and Swagger UI page without adding a NuGet dependency.
- Admin role seed and role assignment endpoint.
- Forgot password and reset password backend/frontend flow.
- Rate limiting for login, refresh, forgot-password, and reset-password.
- Audit logs for register, login, refresh, logout, change-password, reset-password, and role assignment.
- Frontend session auth through Next.js API routes with HttpOnly cookies instead of localStorage.
- Frontend smoke tests for auth pages/session routes.
- Production secret cleanup through `.env` and Compose environment variables.

Build and backend tests:

```bash
dotnet test IdentityService.slnx -c Release
```

Latest result:
- API tests: `1` passed.
- Unit tests: `5` passed.
- Integration tests: `3` passed.

Frontend build and tests:

```bash
cd src/Ecommerce.Web
npm run build
npm test
```

Latest result:
- Next.js production build passed.
- Frontend auth/session tests: `4` passed.

Compose validation:

```bash
docker compose config --quiet
```

```bash
docker compose -f docker-compose.yml -f docker-compose.local-mysql.yml config --quiet
```

Latest result:
- Both Compose modes validate.

Rebuild running stack:

```bash
docker compose up -d --build
```

Apply Phase-1 hardening SQL to Docker MySQL:

```bash
docker compose exec -T mysql mysql -u${MYSQL_USER:-khuntunlar} -p${MYSQL_PASSWORD:-khuntunlar2024} ${MYSQL_DATABASE:-ecommerce_with_dot_net} < src/IdentityService.Infrastructure/Persistence/Migrations/202607130001_Phase01Hardening.mysql.sql
```

Apply Phase-1 hardening SQL to local MySQL:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -u${MYSQL_USER:-khuntunlar} -p${MYSQL_PASSWORD:-khuntunlar2024} ${MYSQL_DATABASE:-ecommerce_with_dot_net} < src/IdentityService.Infrastructure/Persistence/Migrations/202607130001_Phase01Hardening.mysql.sql
```

Verify hardening schema:

```bash
docker compose exec -T mysql mysql -u${MYSQL_USER:-khuntunlar} -p${MYSQL_PASSWORD:-khuntunlar2024} ${MYSQL_DATABASE:-ecommerce_with_dot_net} -e "SHOW TABLES LIKE 'PasswordResetTokens'; SELECT Name, NormalizedName FROM Roles ORDER BY Name;"
```

Latest result:
- `PasswordResetTokens` exists.
- `Admin` and `Customer` roles exist.

Live forgot/reset password verification:

```bash
curl -X POST http://localhost:5294/api/v1/auth/register -H 'Content-Type: application/json' -d '{"displayName":"Phase1 Hardening","email":"phase1-hardening-<timestamp>@tun.shop","password":"admin12345"}'
```

Result: `201`.

```bash
curl -X POST http://localhost:5294/api/v1/auth/forgot-password -H 'Content-Type: application/json' -d '{"email":"phase1-hardening-<timestamp>@tun.shop"}'
```

Result: `200`, with development reset token.

```bash
curl -X POST http://localhost:5294/api/v1/auth/reset-password -H 'Content-Type: application/json' -d '{"email":"phase1-hardening-<timestamp>@tun.shop","resetToken":"<reset-token>","newPassword":"newpass12345"}'
```

Result: `204`.

```bash
curl -X POST http://localhost:5294/api/v1/auth/login -H 'Content-Type: application/json' -d '{"email":"phase1-hardening-<timestamp>@tun.shop","password":"newpass12345"}'
```

Result: `200`.

OpenAPI verification:

```bash
curl http://localhost:5294/openapi/v1.json
```

Result: `200`, title `Identity Service API`.

## Phase 02 Catalog Service Start

Decision:
- Use database-per-service.
- Identity Service database: `identity_service`.
- Catalog Service database: `catalog_service`.

Created Catalog Service solution and projects:

```bash
mkdir -p src/CatalogService.Domain/Common src/CatalogService.Domain/Catalog src/CatalogService.Application/Abstractions src/CatalogService.Application/Common/Behaviors src/CatalogService.Application/Common/Exceptions src/CatalogService.Application/Categories src/CatalogService.Application/Brands src/CatalogService.Application/Products src/CatalogService.Infrastructure/Persistence/Configurations src/CatalogService.Infrastructure/Persistence/Migrations src/CatalogService.Api/Controllers tests/CatalogService.UnitTests tests/CatalogService.IntegrationTests tests/CatalogService.ApiTests
```

Created solution:

```bash
CatalogService.slnx
```

Catalog database migration SQL:

```bash
src/CatalogService.Infrastructure/Persistence/Migrations/202607210001_InitialCatalogSchema.mysql.sql
```

Catalog API Dockerfile:

```bash
src/CatalogService.Api/Dockerfile
```

Updated Docker Compose for separate databases:
- `identity-mysql` -> database `identity_service`, host port `3307`.
- `catalog-mysql` -> database `catalog_service`, host port `3308`.
- `identity-api` -> `http://localhost:5294`.
- `catalog-api` -> `http://localhost:5295`.
- `web` depends on both APIs.

Validate Docker MySQL mode:

```bash
docker compose config --quiet
```

Validate local MySQL mode:

```bash
docker compose -f docker-compose.yml -f docker-compose.local-mysql.yml config --quiet
```

Latest result:
- Both Compose modes validate.

Run Catalog tests:

```bash
dotnet test CatalogService.slnx -c Release
```

Latest result:
- Catalog unit tests: `2` passed.
- Catalog integration tests: `1` passed.
- Catalog API tests: `1` passed.

Attempted Docker rebuild/start:

```bash
docker compose up -d --build
```

Latest result:
- Could not run because Docker daemon was not reachable: `Cannot connect to the Docker daemon at unix:///home/khuntunlar/.docker/desktop/docker.sock`.

Run after Docker daemon/Desktop is started:

```bash
docker compose up -d --build
```

Apply Catalog schema manually if using local MySQL mode:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -u${MYSQL_USER:-khuntunlar} -p${MYSQL_PASSWORD:-khuntunlar2024} ${CATALOG_MYSQL_DATABASE:-catalog_service} < src/CatalogService.Infrastructure/Persistence/Migrations/202607210001_InitialCatalogSchema.mysql.sql
```

Verify Docker Catalog MySQL after Docker is running:

```bash
docker compose exec -T catalog-mysql mysql -u${MYSQL_USER:-khuntunlar} -p${MYSQL_PASSWORD:-khuntunlar2024} ${CATALOG_MYSQL_DATABASE:-catalog_service} -e "SHOW TABLES;"
```

## Phase 02 Catalog Runtime Verification

Re-validated Catalog service after Docker became available.

Catalog tests:

```bash
dotnet test CatalogService.slnx -c Release --no-restore
```

Latest result:
- Catalog unit tests: `2` passed.
- Catalog integration tests: `1` passed.
- Catalog API tests: `1` passed.

Start database-per-service Docker stack:

```bash
docker compose up -d --build
```

Latest result:
- `ecommerce-identity-mysql` healthy on host port `3307`.
- `ecommerce-catalog-mysql` healthy on host port `3308`.
- `ecommerce-identity-api` running on host port `5294`.
- `ecommerce-catalog-api` running on host port `5295`.
- `ecommerce-web` running on host port `3000`.

Note:
- Docker warned about old orphan container `ecommerce-mysql` from the previous single-MySQL setup.
- It can be cleaned later with `docker compose up -d --build --remove-orphans` or explicit container removal if no longer needed.

Health checks:

```bash
curl http://localhost:5295/api/v1/health
```

Result:
- `200 OK`, `{"status":"healthy","service":"catalog"}`.

```bash
curl http://localhost:5294/api/v1/health
```

Result:
- `200 OK`, `{"status":"healthy"}`.

Verify separate database schemas:

```bash
docker compose exec -T catalog-mysql mysql -ukhuntunlar -p<mysql-password> catalog_service -e "SHOW TABLES;"
```

Latest result:
- `Brands`
- `Categories`
- `ProductImages`
- `Products`
- `__EFMigrationsHistory`

```bash
docker compose exec -T identity-mysql mysql -ukhuntunlar -p<mysql-password> identity_service -e "SHOW TABLES;"
```

Latest result:
- `AuditLogs`
- `PasswordResetTokens`
- `RefreshTokens`
- `Roles`
- `UserRoles`
- `Users`
- `__EFMigrationsHistory`

Catalog CRUD smoke test:

```bash
curl -X POST http://localhost:5295/api/v1/categories \
  -H 'Content-Type: application/json' \
  -d '{"name":"Shoes","slug":"shoes","isActive":true}'
```

```bash
curl -X POST http://localhost:5295/api/v1/brands \
  -H 'Content-Type: application/json' \
  -d '{"name":"Tun Brand","slug":"tun-brand","isActive":true}'
```

```bash
curl -X POST http://localhost:5295/api/v1/products \
  -H 'Content-Type: application/json' \
  -d '{"categoryId":"<category-id>","brandId":"<brand-id>","name":"Runner One","slug":"runner-one","description":"A phase two test product","price":59.99,"sku":"RUNNER-ONE","isActive":true}'
```

Latest result:
- Product create returned `201`.
- Product list returned `200`.
- Product get by id returned `200`.

## Phase 02 - Catalog CQRS Refactor Verification

Purpose:
- Move Catalog controller logic into Application commands and queries.
- Keep controllers thin with MediatR.
- Validate requests with FluentValidation pipeline behavior.
- Verify by automated tests and real Docker API calls.

Run Catalog test suite:

```bash
dotnet test CatalogService.slnx -c Release
```

Latest result:
- `CatalogService.IntegrationTests`: Passed `1`.
- `CatalogService.UnitTests`: Passed `8`.
- `CatalogService.ApiTests`: Passed `1`.
- Total: Passed `10`, Failed `0`.

Rebuild and restart Docker stack:

```bash
docker compose up -d --build
```

Latest result:
- `ecommerce-catalog-api` rebuilt and started on host port `5295`.
- `ecommerce-identity-api` rebuilt and started on host port `5294`.
- `ecommerce-catalog-mysql` healthy on host port `3308`.
- `ecommerce-identity-mysql` healthy on host port `3307`.
- `ecommerce-web` running on host port `3000`.

Check Catalog health:

```bash
curl -sS -i http://localhost:5295/api/v1/health
```

Latest result:
- `200 OK`
- `{"status":"healthy","service":"catalog"}`

Create Category through CQRS endpoint:

```bash
curl -sS -X POST http://localhost:5295/api/v1/categories \
  -H 'Content-Type: application/json' \
  -d '{"name":"CQRS Shoes 1786984667","slug":"cqrs-shoes-1786984667","isActive":true}'
```

Latest result:
- Returned category id `86c65bd4-6061-4274-a250-94a873a4f808`.

Create Brand through CQRS endpoint:

```bash
curl -sS -X POST http://localhost:5295/api/v1/brands \
  -H 'Content-Type: application/json' \
  -d '{"name":"CQRS Brand 1786984667","slug":"cqrs-brand-1786984667","isActive":true}'
```

Latest result:
- Returned brand id `6c847cc9-3e02-42d2-ab93-11ec6f822703`.

Create Product through CQRS endpoint:

```bash
curl -sS -X POST http://localhost:5295/api/v1/products \
  -H 'Content-Type: application/json' \
  -d '{"categoryId":"86c65bd4-6061-4274-a250-94a873a4f808","brandId":"6c847cc9-3e02-42d2-ab93-11ec6f822703","name":"CQRS Runner 1786984667","slug":"cqrs-runner-1786984667","description":"CQRS smoke product","price":79.99,"sku":"CQRS-RUNNER-1786984667","isActive":true}'
```

Latest result:
- Returned product id `b594224d-3739-433b-bfa9-72b4fe9cea40`.

Verify validation behavior:

```bash
curl -sS -i -X POST http://localhost:5295/api/v1/categories \
  -H 'Content-Type: application/json' \
  -d '{"name":"","slug":"Bad Slug","isActive":true}'
```

Latest result:
- `400 Bad Request`
- `{"title":"Validation failed","status":400,"detail":"'Name' must not be empty.; 'Slug' is not in the correct format."}`

## Phase 02 - Catalog Admin Authorization Verification

Purpose:
- Keep Catalog read endpoints public.
- Require valid JWT bearer authentication for Catalog write endpoints.
- Require `Admin` role for Catalog `POST`, `PUT`, and `DELETE` actions.
- Reuse the same JWT issuer, audience, and signing key as IdentityService.

Run Catalog test suite:

```bash
dotnet test CatalogService.slnx -c Release
```

Latest result:
- `CatalogService.IntegrationTests`: Passed `1`.
- `CatalogService.UnitTests`: Passed `8`.
- `CatalogService.ApiTests`: Passed `3`.
- Total: Passed `12`, Failed `0`.

Rebuild and restart Docker stack:

```bash
docker compose up -d --build
```

Latest result:
- `ecommerce-catalog-api` rebuilt and started on host port `5295`.
- `ecommerce-identity-api` rebuilt and started on host port `5294`.
- `ecommerce-catalog-mysql` healthy on host port `3308`.
- `ecommerce-identity-mysql` healthy on host port `3307`.
- `ecommerce-web` running on host port `3000`.

Verify public Catalog reads still work:

```bash
curl -sS -i http://localhost:5295/api/v1/categories
```

Latest result:
- `200 OK`

Verify anonymous Catalog writes are blocked:

```bash
curl -sS -i -X POST http://localhost:5295/api/v1/categories \
  -H 'Content-Type: application/json' \
  -d '{"name":"Blocked Category","slug":"blocked-category","isActive":true}'
```

Latest result:
- `401 Unauthorized`

Verify non-admin authenticated writes are forbidden:

```bash
# Create a short-lived local JWT with role claim `Customer`, then call Catalog write endpoint.
curl -sS -i -X POST http://localhost:5295/api/v1/categories \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer <customer-jwt>" \
  -d '{"name":"Customer Category","slug":"customer-category","isActive":true}'
```

Latest result:
- `403 Forbidden`

Verify admin authenticated writes are allowed:

```bash
# Create a short-lived local JWT with role claim `Admin`, then call Catalog write endpoint.
curl -sS -i -X POST http://localhost:5295/api/v1/categories \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer <admin-jwt>" \
  -d '{"name":"Admin Category","slug":"admin-category","isActive":true}'
```

Latest result:
- `201 Created`

## Phase 02 - Catalog Product Images Verification

Purpose:
- Add product image API endpoints under Catalog.
- Keep image list public.
- Require `Admin` JWT role for add, set-primary, and delete.
- Add `IsPrimary` support to `ProductImages` table.

Apply ProductImages migration to existing Docker Catalog MySQL volume:

```bash
docker compose exec -T catalog-mysql mysql \
  -u${MYSQL_USER:-khuntunlar} \
  -p${CATALOG_DB_PASSWORD:-khuntunlar2024} \
  ${CATALOG_MYSQL_DATABASE:-catalog_service} \
  < src/CatalogService.Infrastructure/Persistence/Migrations/202608170001_AddProductImagePrimary.mysql.sql
```

Verify ProductImages schema:

```bash
docker compose exec -T catalog-mysql mysql \
  -u${MYSQL_USER:-khuntunlar} \
  -p${CATALOG_DB_PASSWORD:-khuntunlar2024} \
  ${CATALOG_MYSQL_DATABASE:-catalog_service} \
  -e "SHOW COLUMNS FROM ProductImages; SELECT * FROM __EFMigrationsHistory WHERE MigrationId='202608170001_AddProductImagePrimary';"
```

Latest result:
- `ProductImages.IsPrimary` exists as `tinyint(1) NOT NULL DEFAULT 0`.
- Migration history includes `202608170001_AddProductImagePrimary`.

Run Catalog test suite:

```bash
dotnet test CatalogService.slnx -c Release
```

Latest result:
- `CatalogService.IntegrationTests`: Passed `1`.
- `CatalogService.UnitTests`: Passed `13`.
- `CatalogService.ApiTests`: Passed `3`.
- Total: Passed `17`, Failed `0`.

Rebuild and restart Docker stack:

```bash
docker compose up -d --build
```

Latest result:
- `ecommerce-catalog-api` rebuilt and started on host port `5295`.
- `ecommerce-catalog-mysql` healthy on host port `3308`.
- `ecommerce-identity-api` started on host port `5294`.
- `ecommerce-web` running on host port `3000`.

Create product image as anonymous user:

```bash
curl -sS -i -X POST http://localhost:5295/api/v1/products/<product-id>/images \
  -H 'Content-Type: application/json' \
  -d '{"url":"https://cdn.tun.shop/anonymous.jpg","altText":"Anonymous image","sortOrder":0,"isPrimary":true}'
```

Latest result:
- `401 Unauthorized`

Create product image as Admin:

```bash
curl -sS -X POST http://localhost:5295/api/v1/products/<product-id>/images \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer <admin-jwt>" \
  -d '{"url":"https://cdn.tun.shop/one.jpg","altText":"Image one","sortOrder":1,"isPrimary":true}'
```

Latest result:
- `201 Created`
- Returned image id `6f1e0828-9cb2-4da8-935c-fe06de4c39d6`.

List product images publicly:

```bash
curl -sS -i http://localhost:5295/api/v1/products/<product-id>/images
```

Latest result:
- `200 OK`

Set primary image as Admin:

```bash
curl -sS -i -X PUT http://localhost:5295/api/v1/products/<product-id>/images/<image-id>/primary \
  -H "Authorization: Bearer <admin-jwt>"
```

Latest result:
- `204 No Content`
- Selected image becomes `isPrimary: true`.
- Other images for the same product become `isPrimary: false`.

Delete product image as Admin:

```bash
curl -sS -i -X DELETE http://localhost:5295/api/v1/products/<product-id>/images/<image-id> \
  -H "Authorization: Bearer <admin-jwt>"
```

Latest result:
- `204 No Content`

## Phase 02 - Catalog Product Search And Pagination Verification

Purpose:
- Return products as a paged result instead of an unbounded array.
- Support product search, filters, sorting, and pagination for frontend catalog screens.

Supported query parameters:
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

Run Catalog test suite:

```bash
dotnet test CatalogService.slnx -c Release
```

Latest result:
- `CatalogService.IntegrationTests`: Passed `1`.
- `CatalogService.UnitTests`: Passed `20`.
- `CatalogService.ApiTests`: Passed `3`.
- Total: Passed `24`, Failed `0`.

Rebuild and restart Docker stack:

```bash
docker compose up -d --build
```

Latest result:
- `ecommerce-catalog-api` rebuilt and started on host port `5295`.
- `ecommerce-catalog-mysql` healthy on host port `3308`.
- `ecommerce-identity-api` started on host port `5294`.
- `ecommerce-web` running on host port `3000`.

Verify default pagination:

```bash
curl -sS -i 'http://localhost:5295/api/v1/products?page=1&pageSize=2'
```

Latest result:
- `200 OK`
- Response includes `items`, `page`, `pageSize`, `totalItems`, `totalPages`, `hasPreviousPage`, and `hasNextPage`.
- Example result had `page: 1`, `pageSize: 2`, `totalItems: 3`, `totalPages: 2`, `hasNextPage: true`.

Verify search, price filter, and sorting:

```bash
curl -sS -i 'http://localhost:5295/api/v1/products?search=Runner&minPrice=10&maxPrice=200&sortBy=price&sortDirection=desc&page=1&pageSize=5'
```

Latest result:
- `200 OK`
- Returned matching Runner products sorted by price descending.

Verify validation for bad paging and sorting:

```bash
curl -sS -i 'http://localhost:5295/api/v1/products?page=0&pageSize=101&sortBy=unknown'
```

Latest result:
- `400 Bad Request`
- Validation detail includes invalid `Page`, `PageSize`, and `SortBy` messages.

## Phase 02 - Frontend Catalog Product Listing Verification

Purpose:
- Add public frontend product listing page.
- Fetch live product data from CatalogService through the web server.
- Support search, price filters, sorting, and pagination in URL query parameters.

Run frontend tests:

```bash
cd src/Ecommerce.Web
npm test
```

Latest result:
- `tests/auth-session.test.mjs`: passed.
- `tests/catalog-page.test.mjs`: passed.
- Total: Passed `2`, Failed `0`.

Run production frontend build:

```bash
cd src/Ecommerce.Web
npm run build
```

Latest result:
- Next.js production build succeeded.
- `/products` is dynamic server-rendered.

Rebuild and restart Docker stack:

```bash
docker compose up -d --build
```

Latest result:
- `ecommerce-web` rebuilt and started on host port `3000`.
- `ecommerce-catalog-api` running on host port `5295`.
- `ecommerce-catalog-mysql` healthy on host port `3308`.

Verify product listing page:

```bash
curl -sS -i 'http://localhost:3000/products?page=1&pageSize=2'
```

Latest result:
- `200 OK`
- HTML includes `Browse the shelf before the cart exists.`
- HTML includes product cards and pagination summary.
- Example result showed `3 products found`, page `1` of `2`.

Verify filtered/sorted product listing page:

```bash
curl -sS -i 'http://localhost:3000/products?search=Runner&sortBy=price&sortDirection=desc&page=1&pageSize=3'
```

Latest result:
- `200 OK`
- HTML includes search value `Runner`.
- HTML includes product cards sorted by price descending.

## Phase 02 - Frontend Product Detail Verification

Purpose:
- Add public product detail page at `/products/[id]`.
- Fetch product details and product images from CatalogService through the web server.
- Link product listing cards to product detail pages.

Run frontend tests:

```bash
cd src/Ecommerce.Web
npm test
```

Latest result:
- `tests/auth-session.test.mjs`: passed.
- `tests/catalog-page.test.mjs`: passed.
- Total: Passed `2`, Failed `0`.

Run production frontend build:

```bash
cd src/Ecommerce.Web
npm run build
```

Latest result:
- Next.js production build succeeded.
- `/products` is dynamic server-rendered.
- `/products/[id]` is dynamic server-rendered.

Rebuild and restart Docker stack:

```bash
docker compose up -d --build
```

Latest result:
- `ecommerce-web` rebuilt and started on host port `3000`.
- `ecommerce-catalog-api` running on host port `5295`.
- `ecommerce-catalog-mysql` healthy on host port `3308`.

Get a product id from Catalog API:

```bash
curl -sS 'http://localhost:5295/api/v1/products?page=1&pageSize=1&sortBy=name'
```

Latest result:
- Example product id: `b594224d-3739-433b-bfa9-72b4fe9cea40`.

Verify product detail page:

```bash
curl -sS -i 'http://localhost:3000/products/b594224d-3739-433b-bfa9-72b4fe9cea40'
```

Latest result:
- `200 OK`
- HTML includes `Back to products`.
- HTML includes product price.

Verify image-backed product detail gallery:

```bash
curl -sS -i 'http://localhost:3000/products/1d6372f3-c299-4f2a-9565-a12874a304a7'
```

Latest result:
- `200 OK`
- HTML includes `Gallery`.

## Phase 02 - Admin Catalog UI Verification

Purpose:
- Add protected admin Catalog workspace at `/admin/catalog`.
- Keep admin JWT in httpOnly cookies.
- Use Next.js server route handlers to forward admin Catalog requests to CatalogService.
- Support create, update, and delete forms for Categories, Brands, Products, and Product Images.

Run frontend tests:

```bash
cd src/Ecommerce.Web
npm test
```

Latest result:
- `tests/admin-catalog.test.mjs`: passed.
- `tests/auth-session.test.mjs`: passed.
- `tests/catalog-page.test.mjs`: passed.
- Total: Passed `3`, Failed `0`.

Run production frontend build:

```bash
cd src/Ecommerce.Web
npm run build
```

Latest result:
- Next.js production build succeeded.
- `/admin/catalog` is dynamic server-rendered.
- Admin Catalog proxy routes are dynamic route handlers.

Rebuild and restart Docker stack:

```bash
docker compose up -d --build
```

Latest result:
- `ecommerce-web` rebuilt and started on host port `3000`.
- `ecommerce-catalog-api` running on host port `5295`.
- `ecommerce-identity-api` running on host port `5294`.

Verify unauthenticated admin page redirect:

```bash
curl -sS -i 'http://localhost:3000/admin/catalog'
```

Latest result:
- `307 Temporary Redirect`
- `location: /login?next=/admin/catalog`

Verify unauthenticated admin proxy rejection:

```bash
curl -sS -i 'http://localhost:3000/api/admin/catalog/categories'
```

Latest result:
- `401 Unauthorized`
- `{"title":"Authentication failed","detail":"Missing session."}`

Verify admin page with short-lived dev Admin JWT cookie:

```bash
curl -sS -i 'http://localhost:3000/admin/catalog' \
  -H 'Cookie: ecommerce_access_token=<admin-jwt>; ecommerce_user=<url-encoded-admin-user-json>'
```

Latest result:
- `200 OK`
- HTML includes `Admin Catalog`.

Verify admin proxy create/delete using short-lived dev Admin JWT cookie:

```bash
curl -sS -i -X POST 'http://localhost:3000/api/admin/catalog/categories' \
  -H 'Content-Type: application/json' \
  -H 'Cookie: ecommerce_access_token=<admin-jwt>; ecommerce_user=<url-encoded-admin-user-json>' \
  -d '{"name":"Admin UI Smoke","slug":"admin-ui-smoke","isActive":true}'
```

Latest result:
- `201 Created`
- Example category id: `35314e27-d632-406d-add2-cdaf29203c72`.

```bash
curl -sS -i -X DELETE 'http://localhost:3000/api/admin/catalog/categories/35314e27-d632-406d-add2-cdaf29203c72' \
  -H 'Cookie: ecommerce_access_token=<admin-jwt>; ecommerce_user=<url-encoded-admin-user-json>'
```

Latest result:
- `204 No Content`

## Phase 02 - Catalog OpenAPI Verification

Purpose:
- Add Catalog OpenAPI JSON at `/openapi/v1.json`.
- Add Swagger UI HTML at `/swagger`.
- Document public read endpoints and Admin-only write endpoints.
- Include schemas for Categories, Brands, Products, paged product search, and Product Images.

Run Catalog test suite:

```bash
dotnet test CatalogService.slnx -c Release
```

Latest result:
- `CatalogService.IntegrationTests`: Passed `1`.
- `CatalogService.UnitTests`: Passed `20`.
- `CatalogService.ApiTests`: Passed `4`.
- Total: Passed `25`, Failed `0`.

Rebuild and restart Docker stack:

```bash
docker compose up -d --build
```

Latest result:
- `ecommerce-catalog-api` rebuilt and started on host port `5295`.
- `ecommerce-web` running on host port `3000`.
- `ecommerce-identity-api` running on host port `5294`.

Verify Catalog OpenAPI JSON:

```bash
curl -sS -i http://localhost:5295/openapi/v1.json
```

Latest result:
- `200 OK`
- Response title: `Catalog Service API`.

Verify key OpenAPI content:

```bash
curl -sS http://localhost:5295/openapi/v1.json \
  | rg 'Catalog Service API|/api/v1/products|/api/v1/products/\{productId\}/images|PagedProductResult|Bearer|Admin role required'
```

Latest result:
- OpenAPI JSON includes product endpoints.
- OpenAPI JSON includes product image endpoints.
- OpenAPI JSON includes `PagedProductResult` schema.
- OpenAPI JSON includes `Bearer` security scheme.
- OpenAPI JSON marks write operations with `Admin role required`.

Verify Swagger UI HTML:

```bash
curl -sS -i http://localhost:5295/swagger
```

Latest result:
- `200 OK`
- HTML includes `Catalog Service API` and Swagger UI bundle loading `/openapi/v1.json`.

## Phase 02 - Specification Checkpoint

Purpose:
- Review Phase-2 implementation against the project specification.
- Update Phase-2 docs from first-slice planning notes to current completed implementation status.
- Add a Phase-2 status checkpoint before moving to the next phase.

Reviewed files:

```bash
rg --files doc project-specification 2>/dev/null | sort | sed -n '1,160p'
rg -n "Phase 02|Phase-02|Catalog|Product|remaining|status|TODO|OpenAPI|Swagger|Admin|Product Image|pagination|search" doc project-specification -g '*.md' 2>/dev/null | sed -n '1,220p'
sed -n '1,220p' doc/project-specification/Phase-02-Product/README.md
sed -n '1,240p' doc/project-specification/Phase-02-Product/API.md
sed -n '1,220p' doc/project-specification/Phase-02-Product/Backend.md
sed -n '1,220p' doc/project-specification/Phase-02-Product/Database.md
sed -n '1,220p' doc/project-specification/Phase-02-Product/Frontend.md
sed -n '1,220p' doc/project-specification/Phase-02-Product/Testing.md
```

Updated documentation:
- `doc/project-specification/Phase-02-Product/README.md`
- `doc/project-specification/Phase-02-Product/API.md`
- `doc/project-specification/Phase-02-Product/Backend.md`
- `doc/project-specification/Phase-02-Product/Database.md`
- `doc/project-specification/Phase-02-Product/Frontend.md`
- `doc/project-specification/Phase-02-Product/Testing.md`
- `doc/project-specification/Phase-02-Product/STATUS.md`

Verification after Phase-2 specification checkpoint:

```bash
cd src/Ecommerce.Web
npm test
npm run build
```

Results:
- `npm test`: passed `3` frontend tests.
- `npm run build`: passed. Next.js built `/products`, `/products/[id]`, `/admin/catalog`, and admin proxy routes.

Backend verification attempted:

```bash
dotnet test CatalogService.slnx -c Release --no-restore -m:1 /nr:false -p:UseSharedCompilation=false
```

Result:
- Catalog projects compiled successfully.
- VSTest execution was blocked by the local sandbox because it could not open its internal TCP listener: `System.Net.Sockets.SocketException (13): Permission denied`.
- Previous unrestricted Catalog verification passed before this documentation-only checkpoint: integration `1`, unit `20`, API `4`, total `25` tests.


## Local MySQL Unknown Database Fix

Issue seen in logs:
- `MySqlConnector.MySqlException: Unknown database 'identity_service'`
- API connection string was using `Server=host.docker.internal;Port=3306;Database=identity_service`, which means Docker local-MySQL mode was active.
- The local machine MySQL server did not have the `identity_service` database prepared.

Fix implemented:
- Updated `docker-compose.local-mysql.yml` to add a one-shot `local-mysql-init` service.
- The init service waits for local MySQL, creates `identity_service` and `catalog_service`, and applies Identity/Catalog migration SQL before API containers start.
- Updated local override dependencies so `identity-api` and `catalog-api` wait for `local-mysql-init` instead of Docker MySQL containers.
- Updated Identity local development connection string from old `ecommerce_with_dot_net` to `identity_service`.
- Muted the development/test MediatR license warning log category with `LuckyPennySoftware.MediatR.License: None`.

Validation commands:

```bash
docker compose -f docker-compose.yml -f docker-compose.local-mysql.yml config --quiet
dotnet build IdentityService.slnx -c Release --no-restore -m:1 /nr:false -p:UseSharedCompilation=false
```

Results:
- Compose local-MySQL override config validation passed.
- IdentityService build passed with `0` warnings and `0` errors.

Run local MySQL mode:

```bash
docker compose -f docker-compose.yml -f docker-compose.local-mysql.yml up --build
```

Manual fallback if local MySQL user cannot create databases from the init service:

```bash
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p -e "CREATE DATABASE IF NOT EXISTS identity_service CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci; CREATE DATABASE IF NOT EXISTS catalog_service CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;"
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p identity_service < src/IdentityService.Infrastructure/Persistence/Migrations/202607030001_InitialIdentitySchema.mysql.sql
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p identity_service < src/IdentityService.Infrastructure/Persistence/Migrations/202607130001_Phase01Hardening.mysql.sql
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p catalog_service < src/CatalogService.Infrastructure/Persistence/Migrations/202607210001_InitialCatalogSchema.mysql.sql
mysql --protocol=TCP -h 127.0.0.1 -P 3306 -ukhuntunlar -p catalog_service < src/CatalogService.Infrastructure/Persistence/Migrations/202608170001_AddProductImagePrimary.mysql.sql
```


## Local MySQL Fix Runtime Verification

Started local MySQL mode after adding the local init service:

```bash
docker compose -f docker-compose.yml -f docker-compose.local-mysql.yml up -d --build
```

First retry result:
- API images built successfully.
- `local-mysql-init` failed because local MySQL did not support MySQL 8 collation `utf8mb4_0900_ai_ci`.

Fix:
- Changed local database creation collation to `utf8mb4_unicode_ci` for better MySQL/MariaDB compatibility.

Successful rerun result:
- `local-mysql-init` completed successfully.
- `identity-api`, `catalog-api`, and `web` started.
- Init logs showed: `Local MySQL databases are ready: identity_service, catalog_service`.

Status/log checks:

```bash
docker compose -f docker-compose.yml -f docker-compose.local-mysql.yml ps
docker logs ecommerce-local-mysql-init --tail 80
docker logs ecommerce-identity-api --tail 200 2>&1 | rg -n "Unknown database|MediatR.License|fail:|Request failed" || true
```

Results:
- `ecommerce-identity-api` running on `5294 -> 8080`.
- `ecommerce-catalog-api` running on `5295 -> 8080`.
- `ecommerce-web` running on `3000 -> 3000`.
- No `Unknown database` logs after fix.
- No `LuckyPennySoftware.MediatR.License` logs after logging filter update.

Verified Compose-network health from the web container:

```bash
docker exec ecommerce-web sh -ec 'wget -qO- http://identity-api:8080/api/v1/health; echo; wget -qO- http://catalog-api:8080/api/v1/health; echo'
```

Results:
- Identity health: `{"status":"healthy"}`.
- Catalog health: `{"status":"healthy","service":"catalog"}`.

Verified register/login against local MySQL mode from the web container:

```bash
docker exec ecommerce-web node -e '<register-and-login-smoke-test>'
```

Results:
- Register returned `201`.
- Login returned `200`.
- Created user email: `local-smoke-1786990749512@tun.shop`.

Verified user was stored in local MySQL:

```bash
docker run --rm --add-host host.docker.internal:host-gateway -e MYSQL_PWD='khuntunlar2024' mysql:8.4 sh -ec 'mysql -h host.docker.internal -P 3306 -ukhuntunlar identity_service -e "SELECT Email, DisplayName, IsActive FROM Users ORDER BY CreatedAt DESC LIMIT 3;"'
```

Result:
- Latest user was present in local MySQL `identity_service.Users`.

