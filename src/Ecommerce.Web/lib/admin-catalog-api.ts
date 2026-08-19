export type Category = { id: string; name: string; slug: string; isActive: boolean };
export type Brand = { id: string; name: string; slug: string; isActive: boolean };
export type Product = {
  id: string;
  categoryId: string;
  brandId: string;
  name: string;
  slug: string;
  description: string;
  price: number;
  sku: string;
  isActive: boolean;
};
export type ProductImage = { id: string; productId: string; url: string; altText: string; sortOrder: number; isPrimary: boolean };
export type PagedResult<T> = { items: T[]; page: number; pageSize: number; totalItems: number; totalPages: number; hasPreviousPage: boolean; hasNextPage: boolean };

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  const response = await fetch(`/api/admin/catalog${path}`, {
    ...init,
    headers: { "Content-Type": "application/json", ...init.headers }
  });

  if (!response.ok) {
    const body = await response.json().catch(() => null);
    throw new Error(body?.detail ? `${body.title}: ${body.detail}` : body?.title ?? `Request failed with status ${response.status}`);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

export const adminCatalogApi = {
  categories: () => request<Category[]>("/categories"),
  createCategory: (payload: Omit<Category, "id">) => request<Category>("/categories", { method: "POST", body: JSON.stringify(payload) }),
  updateCategory: (id: string, payload: Omit<Category, "id">) => request<void>(`/categories/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  deleteCategory: (id: string) => request<void>(`/categories/${id}`, { method: "DELETE" }),
  brands: () => request<Brand[]>("/brands"),
  createBrand: (payload: Omit<Brand, "id">) => request<Brand>("/brands", { method: "POST", body: JSON.stringify(payload) }),
  updateBrand: (id: string, payload: Omit<Brand, "id">) => request<void>(`/brands/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  deleteBrand: (id: string) => request<void>(`/brands/${id}`, { method: "DELETE" }),
  products: () => request<PagedResult<Product>>("/products?page=1&pageSize=100"),
  createProduct: (payload: Omit<Product, "id">) => request<Product>("/products", { method: "POST", body: JSON.stringify(payload) }),
  updateProduct: (id: string, payload: Omit<Product, "id">) => request<void>(`/products/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  deleteProduct: (id: string) => request<void>(`/products/${id}`, { method: "DELETE" }),
  productImages: (productId: string) => request<ProductImage[]>(`/products/${productId}/images`),
  addProductImage: (productId: string, payload: Omit<ProductImage, "id" | "productId">) => request<ProductImage>(`/products/${productId}/images`, { method: "POST", body: JSON.stringify(payload) }),
  setPrimaryImage: (productId: string, imageId: string) => request<void>(`/products/${productId}/images/${imageId}/primary`, { method: "PUT" }),
  deleteProductImage: (productId: string, imageId: string) => request<void>(`/products/${productId}/images/${imageId}`, { method: "DELETE" })
};
