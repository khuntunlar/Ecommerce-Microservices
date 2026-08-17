# Product Database

Database name: `catalog_service`

## Tables
- `Categories`
- `Brands`
- `Products`
- `ProductImages`
- `__EFMigrationsHistory`

## Indexes
- Unique category slug
- Unique brand slug
- Unique product slug
- Unique product SKU
- Product category index
- Product brand index

## Migration SQL
- `src/CatalogService.Infrastructure/Persistence/Migrations/202607210001_InitialCatalogSchema.mysql.sql`
- `src/CatalogService.Infrastructure/Persistence/Migrations/202608170001_AddProductImagePrimary.mysql.sql`

## Database-Per-Service Rule
- Identity data is stored in `identity_service`.
- Catalog data is stored in `catalog_service`.
- Catalog should not create tables in the Identity database.
- Future Inventory, Cart, Checkout, Payment, Order, Shipping, Notification, and Reporting phases should continue the same ownership rule.
