import Link from "next/link";
import type React from "react";
import { getProducts, type ProductSearchParams } from "@/lib/server/catalog-api";

type ProductsPageProps = {
  searchParams: Promise<ProductSearchParams>;
};

const sortOptions = [
  { value: "name", label: "Name" },
  { value: "price", label: "Price" },
  { value: "createdAt", label: "Newest" },
  { value: "sku", label: "SKU" }
];

export default async function ProductsPage({ searchParams }: ProductsPageProps) {
  const params = await searchParams;
  const products = await getProducts(params);
  const page = products.page;

  return (
    <main className="catalog-shell">
      <section className="catalog-hero">
        <p className="eyebrow">Phase 02 Catalog</p>
        <div>
          <h1>Browse the shelf before the cart exists.</h1>
          <p>
            Public product discovery is now connected to the Catalog Service with live search,
            sorting, price filters, and pagination.
          </p>
        </div>
      </section>

      <form className="catalog-filters" action="/products">
        <label>
          Search
          <input name="search" type="search" placeholder="Runner, SKU, description" defaultValue={params.search ?? ""} />
        </label>
        <label>
          Min price
          <input name="minPrice" type="number" min="0" step="0.01" defaultValue={params.minPrice ?? ""} />
        </label>
        <label>
          Max price
          <input name="maxPrice" type="number" min="0" step="0.01" defaultValue={params.maxPrice ?? ""} />
        </label>
        <label>
          Sort by
          <select name="sortBy" defaultValue={params.sortBy ?? "name"}>
            {sortOptions.map((option) => <option key={option.value} value={option.value}>{option.label}</option>)}
          </select>
        </label>
        <label>
          Direction
          <select name="sortDirection" defaultValue={params.sortDirection ?? "asc"}>
            <option value="asc">Ascending</option>
            <option value="desc">Descending</option>
          </select>
        </label>
        <input name="pageSize" type="hidden" value={params.pageSize ?? "9"} />
        <button className="button primary" type="submit">Search catalog</button>
        <Link className="button secondary" href="/products">Clear</Link>
      </form>

      <section className="catalog-summary" aria-live="polite">
        <p>{products.totalItems} product{products.totalItems === 1 ? "" : "s"} found</p>
        <p>Page {products.totalPages === 0 ? 0 : page} of {products.totalPages}</p>
      </section>

      {products.items.length > 0 ? (
        <section className="product-grid">
          {products.items.map((product, index) => (
            <Link className="product-card" href={`/products/${product.id}`} key={product.id} style={{ "--card-index": index } as React.CSSProperties}>
              <div className="product-art" aria-hidden="true">
                <span>{product.name.slice(0, 2).toUpperCase()}</span>
              </div>
              <div className="product-card-body">
                <p className="product-sku">{product.sku}</p>
                <h2>{product.name}</h2>
                <p>{product.description}</p>
                <div className="product-card-footer">
                  <strong>${product.price.toFixed(2)}</strong>
                  <span>{product.isActive ? "Available" : "Hidden"}</span>
                </div>
              </div>
            </Link>
          ))}
        </section>
      ) : (
        <section className="empty-catalog">
          <p className="eyebrow">No matches</p>
          <h2>No products matched this search.</h2>
          <Link className="button primary" href="/products">Reset filters</Link>
        </section>
      )}

      <nav className="pager" aria-label="Product pages">
        <PageLink disabled={!products.hasPreviousPage} page={page - 1} params={params}>Previous</PageLink>
        <PageLink disabled={!products.hasNextPage} page={page + 1} params={params}>Next</PageLink>
      </nav>
    </main>
  );
}

function PageLink({ children, disabled, page, params }: Readonly<{
  children: React.ReactNode;
  disabled: boolean;
  page: number;
  params: ProductSearchParams;
}>) {
  if (disabled) {
    return <span className="button secondary disabled-link">{children}</span>;
  }

  const hrefParams = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    if (typeof value === "string" && value.trim() && key !== "page") {
      hrefParams.set(key, value.trim());
    }
  }
  hrefParams.set("page", String(page));
  hrefParams.set("pageSize", params.pageSize?.trim() || "9");

  return <Link className="button secondary" href={`/products?${hrefParams.toString()}`}>{children}</Link>;
}
