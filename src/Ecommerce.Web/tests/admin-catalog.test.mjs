import assert from "node:assert/strict";
import { existsSync, readFileSync } from "node:fs";
import { join } from "node:path";
import test from "node:test";

const root = process.cwd();

const requiredFiles = [
  "app/admin/catalog/page.tsx",
  "app/admin/catalog/workspace.tsx",
  "lib/admin-catalog-api.ts",
  "lib/server/catalog-admin-api.ts",
  "app/api/admin/catalog/categories/route.ts",
  "app/api/admin/catalog/brands/route.ts",
  "app/api/admin/catalog/products/route.ts"
];

test("admin catalog files exist", () => {
  for (const file of requiredFiles) {
    assert.equal(existsSync(join(root, file)), true, `${file} should exist`);
  }
});

test("admin page requires Admin role", () => {
  const content = readFileSync(join(root, "app/admin/catalog/page.tsx"), "utf8");
  assert.equal(content.includes("readSessionUser"), true);
  assert.equal(content.includes("roles.includes(\"Admin\")"), true);
});

test("admin proxy forwards bearer token from server cookie", () => {
  const content = readFileSync(join(root, "lib/server/catalog-admin-api.ts"), "utf8");
  assert.equal(content.includes("readAccessToken"), true);
  assert.equal(content.includes("Authorization: `Bearer ${accessToken}`"), true);
});

test("admin workspace supports product images", () => {
  const content = readFileSync(join(root, "app/admin/catalog/workspace.tsx"), "utf8");
  for (const expected of ["addProductImage", "setPrimaryImage", "deleteProductImage"]) {
    assert.equal(content.includes(expected), true, `workspace should include ${expected}`);
  }
});
