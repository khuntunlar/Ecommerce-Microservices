import assert from "node:assert/strict";
import { existsSync, readFileSync } from "node:fs";
import { join } from "node:path";
import test from "node:test";

const root = process.cwd();

const requiredCatalogFiles = [
  "app/products/page.tsx",
  "app/products/[id]/page.tsx",
  "lib/server/catalog-api.ts"
];

test("catalog product page files exist", () => {
  for (const file of requiredCatalogFiles) {
    assert.equal(existsSync(join(root, file)), true, `${file} should exist`);
  }
});

test("product page uses paged catalog search params", () => {
  const content = readFileSync(join(root, "app/products/page.tsx"), "utf8");

  for (const expected of ["search", "minPrice", "maxPrice", "sortBy", "sortDirection", "pageSize"]) {
    assert.equal(content.includes(expected), true, `products page should include ${expected}`);
  }
});

test("catalog server helper targets catalog API", () => {
  const content = readFileSync(join(root, "lib/server/catalog-api.ts"), "utf8");

  assert.equal(content.includes("CATALOG_API_URL"), true);
  assert.equal(content.includes("/api/v1/products"), true);
  assert.equal(content.includes("cache: \"no-store\""), true);
});


test("product list links to detail pages", () => {
  const content = readFileSync(join(root, "app/products/page.tsx"), "utf8");

  assert.equal(content.includes("/products/${product.id}"), true);
});

test("product detail page uses product images", () => {
  const content = readFileSync(join(root, "app/products/[id]/page.tsx"), "utf8");

  assert.equal(content.includes("getProductDetail"), true);
  assert.equal(content.includes("primaryImage"), true);
  assert.equal(content.includes("image-gallery"), true);
});

test("catalog helper fetches product detail and images", () => {
  const content = readFileSync(join(root, "lib/server/catalog-api.ts"), "utf8");

  assert.equal(content.includes("getProductDetail"), true);
  assert.equal(content.includes("/images"), true);
  assert.equal(content.includes("Promise.all"), true);
});
