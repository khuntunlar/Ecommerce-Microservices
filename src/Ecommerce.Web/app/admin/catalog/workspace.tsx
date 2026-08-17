"use client";

import { FormEvent, useEffect, useMemo, useState } from "react";
import { adminCatalogApi, type Brand, type Category, type Product, type ProductImage } from "@/lib/admin-catalog-api";

type Props = { userName: string };
type EditableKind = "category" | "brand" | "product";

export function AdminCatalogWorkspace({ userName }: Props) {
  const [categories, setCategories] = useState<Category[]>([]);
  const [brands, setBrands] = useState<Brand[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [images, setImages] = useState<ProductImage[]>([]);
  const [selectedProductId, setSelectedProductId] = useState("");
  const [status, setStatus] = useState("Loading catalog admin data...");
  const [error, setError] = useState<string | null>(null);
  const [editing, setEditing] = useState<{ kind: EditableKind; id: string } | null>(null);

  const selectedProduct = useMemo(() => products.find((product) => product.id === selectedProductId), [products, selectedProductId]);

  async function loadAll() {
    setError(null);
    const [nextCategories, nextBrands, productPage] = await Promise.all([
      adminCatalogApi.categories(),
      adminCatalogApi.brands(),
      adminCatalogApi.products()
    ]);
    setCategories(nextCategories);
    setBrands(nextBrands);
    setProducts(productPage.items);
    setSelectedProductId((current) => current || productPage.items[0]?.id || "");
    setStatus("Catalog admin data loaded.");
  }

  useEffect(() => {
    loadAll().catch((caught) => setError(caught instanceof Error ? caught.message : "Could not load catalog admin data"));
  }, []);

  useEffect(() => {
    if (!selectedProductId) {
      setImages([]);
      return;
    }

    adminCatalogApi.productImages(selectedProductId)
      .then(setImages)
      .catch((caught) => setError(caught instanceof Error ? caught.message : "Could not load product images"));
  }, [selectedProductId]);

  async function run(action: () => Promise<unknown>, success: string) {
    setError(null);
    setStatus("Saving...");
    try {
      await action();
      await loadAll();
      if (selectedProductId) {
        setImages(await adminCatalogApi.productImages(selectedProductId));
      }
      setEditing(null);
      setStatus(success);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Catalog admin action failed");
      setStatus("Action failed.");
    }
  }

  function categoryPayload(form: HTMLFormElement) {
    const data = new FormData(form);
    return { name: text(data, "name"), slug: text(data, "slug"), isActive: data.get("isActive") === "on" };
  }

  function productPayload(form: HTMLFormElement) {
    const data = new FormData(form);
    return {
      categoryId: text(data, "categoryId"),
      brandId: text(data, "brandId"),
      name: text(data, "name"),
      slug: text(data, "slug"),
      description: text(data, "description"),
      price: Number(data.get("price") || 0),
      sku: text(data, "sku"),
      isActive: data.get("isActive") === "on"
    };
  }

  return (
    <main className="admin-shell">
      <section className="admin-hero">
        <p className="eyebrow">Admin Catalog</p>
        <h1>Shape the shelf, {userName}.</h1>
        <p>Create, update, and delete Catalog data through protected admin routes. The browser never sees the JWT; the Next server forwards it from httpOnly cookies.</p>
        <div className="admin-status">{error ? <span className="form-error">{error}</span> : <span className="form-success">{status}</span>}</div>
      </section>

      <section className="admin-grid">
        <AdminPanel title="Categories" count={categories.length}>
          <EntityForm submitLabel="Create category" onSubmit={(event) => { event.preventDefault(); void run(() => adminCatalogApi.createCategory(categoryPayload(event.currentTarget)), "Category created."); }} />
          <EntityList items={categories} kind="category" editing={editing} onEdit={setEditing} onDelete={(id) => run(() => adminCatalogApi.deleteCategory(id), "Category deleted.")} onUpdate={(id, form) => run(() => adminCatalogApi.updateCategory(id, categoryPayload(form)), "Category updated.")} />
        </AdminPanel>

        <AdminPanel title="Brands" count={brands.length}>
          <EntityForm submitLabel="Create brand" onSubmit={(event) => { event.preventDefault(); void run(() => adminCatalogApi.createBrand(categoryPayload(event.currentTarget)), "Brand created."); }} />
          <EntityList items={brands} kind="brand" editing={editing} onEdit={setEditing} onDelete={(id) => run(() => adminCatalogApi.deleteBrand(id), "Brand deleted.")} onUpdate={(id, form) => run(() => adminCatalogApi.updateBrand(id, categoryPayload(form)), "Brand updated.")} />
        </AdminPanel>
      </section>

      <section className="admin-panel wide-panel">
        <div className="panel-heading"><h2>Products</h2><span>{products.length}</span></div>
        <form className="admin-form product-form" onSubmit={(event) => { event.preventDefault(); void run(() => adminCatalogApi.createProduct(productPayload(event.currentTarget)), "Product created."); }}>
          <ProductFields categories={categories} brands={brands} />
          <button className="button primary" type="submit">Create product</button>
        </form>
        <div className="admin-list product-list">
          {products.map((product) => (
            <article key={product.id} className="admin-row">
              {editing?.kind === "product" && editing.id === product.id ? (
                <form className="admin-form product-form" onSubmit={(event) => { event.preventDefault(); void run(() => adminCatalogApi.updateProduct(product.id, productPayload(event.currentTarget)), "Product updated."); }}>
                  <ProductFields categories={categories} brands={brands} product={product} />
                  <button className="button primary" type="submit">Save product</button>
                  <button className="button secondary" type="button" onClick={() => setEditing(null)}>Cancel</button>
                </form>
              ) : (
                <>
                  <div><strong>{product.name}</strong><span>{product.sku} · ${product.price.toFixed(2)}</span></div>
                  <div className="row-actions">
                    <button className="button secondary" type="button" onClick={() => { setSelectedProductId(product.id); setEditing({ kind: "product", id: product.id }); }}>Edit</button>
                    <button className="button secondary" type="button" onClick={() => setSelectedProductId(product.id)}>Images</button>
                    <button className="button secondary danger" type="button" onClick={() => void run(() => adminCatalogApi.deleteProduct(product.id), "Product deleted.")}>Delete</button>
                  </div>
                </>
              )}
            </article>
          ))}
        </div>
      </section>

      <section className="admin-panel wide-panel">
        <div className="panel-heading"><h2>Product Images</h2><span>{images.length}</span></div>
        <label className="admin-select-label">Selected product
          <select value={selectedProductId} onChange={(event) => setSelectedProductId(event.target.value)}>
            {products.map((product) => <option key={product.id} value={product.id}>{product.name}</option>)}
          </select>
        </label>
        {selectedProduct && (
          <form className="admin-form image-form" onSubmit={(event) => {
            event.preventDefault();
            const data = new FormData(event.currentTarget);
            void run(() => adminCatalogApi.addProductImage(selectedProduct.id, { url: text(data, "url"), altText: text(data, "altText"), sortOrder: Number(data.get("sortOrder") || 0), isPrimary: data.get("isPrimary") === "on" }), "Product image added.");
          }}>
            <label>Image URL<input name="url" type="url" required /></label>
            <label>Alt text<input name="altText" required /></label>
            <label>Sort order<input name="sortOrder" type="number" min="0" defaultValue="0" required /></label>
            <label className="checkbox-label"><input name="isPrimary" type="checkbox" /> Primary</label>
            <button className="button primary" type="submit">Add image</button>
          </form>
        )}
        <div className="image-admin-grid">
          {images.map((image) => (
            <article className="gallery-card" key={image.id}>
              <img src={image.url} alt={image.altText} />
              <div><strong>{image.altText}</strong><span>{image.isPrimary ? "Primary" : `Sort ${image.sortOrder}`}</span></div>
              <div className="row-actions image-actions">
                <button className="button secondary" type="button" onClick={() => void run(() => adminCatalogApi.setPrimaryImage(image.productId, image.id), "Primary image updated.")}>Set primary</button>
                <button className="button secondary danger" type="button" onClick={() => void run(() => adminCatalogApi.deleteProductImage(image.productId, image.id), "Product image deleted.")}>Delete</button>
              </div>
            </article>
          ))}
        </div>
      </section>
    </main>
  );
}

function AdminPanel({ title, count, children }: { title: string; count: number; children: React.ReactNode }) {
  return <section className="admin-panel"><div className="panel-heading"><h2>{title}</h2><span>{count}</span></div>{children}</section>;
}

function EntityForm({ submitLabel, onSubmit }: { submitLabel: string; onSubmit: (event: FormEvent<HTMLFormElement>) => void }) {
  return <form className="admin-form" onSubmit={onSubmit}><EntityFields /><button className="button primary" type="submit">{submitLabel}</button></form>;
}

function EntityFields({ item }: { item?: Category | Brand }) {
  return <><label>Name<input name="name" defaultValue={item?.name ?? ""} required /></label><label>Slug<input name="slug" defaultValue={item?.slug ?? ""} pattern="[a-z0-9-]+" required /></label><label className="checkbox-label"><input name="isActive" type="checkbox" defaultChecked={item?.isActive ?? true} /> Active</label></>;
}

function EntityList({ items, kind, editing, onEdit, onDelete, onUpdate }: { items: (Category | Brand)[]; kind: EditableKind; editing: { kind: EditableKind; id: string } | null; onEdit: (value: { kind: EditableKind; id: string } | null) => void; onDelete: (id: string) => Promise<unknown>; onUpdate: (id: string, form: HTMLFormElement) => Promise<unknown>; }) {
  return <div className="admin-list">{items.map((item) => <article className="admin-row" key={item.id}>{editing?.kind === kind && editing.id === item.id ? <form className="admin-form" onSubmit={(event) => { event.preventDefault(); void onUpdate(item.id, event.currentTarget); }}><EntityFields item={item} /><button className="button primary" type="submit">Save</button><button className="button secondary" type="button" onClick={() => onEdit(null)}>Cancel</button></form> : <><div><strong>{item.name}</strong><span>{item.slug}</span></div><div className="row-actions"><button className="button secondary" type="button" onClick={() => onEdit({ kind, id: item.id })}>Edit</button><button className="button secondary danger" type="button" onClick={() => void onDelete(item.id)}>Delete</button></div></>}</article>)}</div>;
}

function ProductFields({ categories, brands, product }: { categories: Category[]; brands: Brand[]; product?: Product }) {
  return <><label>Category<select name="categoryId" defaultValue={product?.categoryId ?? categories[0]?.id} required>{categories.map((category) => <option key={category.id} value={category.id}>{category.name}</option>)}</select></label><label>Brand<select name="brandId" defaultValue={product?.brandId ?? brands[0]?.id} required>{brands.map((brand) => <option key={brand.id} value={brand.id}>{brand.name}</option>)}</select></label><label>Name<input name="name" defaultValue={product?.name ?? ""} required /></label><label>Slug<input name="slug" defaultValue={product?.slug ?? ""} pattern="[a-z0-9-]+" required /></label><label>Description<textarea name="description" defaultValue={product?.description ?? ""} required /></label><label>Price<input name="price" type="number" step="0.01" min="0" defaultValue={product?.price ?? 0} required /></label><label>SKU<input name="sku" defaultValue={product?.sku ?? ""} required /></label><label className="checkbox-label"><input name="isActive" type="checkbox" defaultChecked={product?.isActive ?? true} /> Active</label></>;
}

function text(data: FormData, key: string) {
  return String(data.get(key) ?? "").trim();
}
