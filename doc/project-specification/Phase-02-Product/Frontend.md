# Product Frontend

Next.js 16 App Router integration for Catalog Service.

## Public Pages
- `/products` lists catalog products from Catalog Service.
- `/products/{id}` shows product details and product images.

## Admin Pages
- `/admin/catalog` is protected by the auth session.
- Admin Catalog UI supports categories, brands, products, and product images.
- Admin write actions call internal Next.js route handlers.
- Internal admin route handlers forward the session JWT to Catalog Service as a Bearer token.

## Server Integration
- Public catalog reads use `src/Ecommerce.Web/lib/server/catalog-api.ts`.
- Admin catalog writes use `src/Ecommerce.Web/lib/server/catalog-admin-api.ts`.
- Browser admin actions use `src/Ecommerce.Web/lib/admin-catalog-api.ts`.
