const catalogApiUrl = process.env.CATALOG_API_URL
  ?? process.env.NEXT_PUBLIC_CATALOG_API_URL
  ?? "http://localhost:5295";

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

export type ProductImage = {
  id: string;
  productId: string;
  url: string;
  altText: string;
  sortOrder: number;
  isPrimary: boolean;
};

export type ProductDetail = {
  product: Product;
  images: ProductImage[];
};

export type PagedResult<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
};

export type ProductSearchParams = {
  search?: string;
  minPrice?: string;
  maxPrice?: string;
  sortBy?: string;
  sortDirection?: string;
  page?: string;
  pageSize?: string;
};

export async function getProducts(params: ProductSearchParams): Promise<PagedResult<Product>> {
  const url = catalogUrl("/api/v1/products");
  const query = new URLSearchParams();

  for (const [key, value] of Object.entries(params)) {
    if (typeof value === "string" && value.trim()) {
      query.set(key, value.trim());
    }
  }

  if (!query.has("page")) {
    query.set("page", "1");
  }

  if (!query.has("pageSize")) {
    query.set("pageSize", "9");
  }

  url.search = query.toString();

  return catalogJson<PagedResult<Product>>(url);
}

export async function getProductDetail(id: string): Promise<ProductDetail> {
  const [product, images] = await Promise.all([
    catalogJson<Product>(catalogUrl(`/api/v1/products/${id}`)),
    catalogJson<ProductImage[]>(catalogUrl(`/api/v1/products/${id}/images`))
  ]);

  return { product, images };
}

function catalogUrl(path: string) {
  return new URL(path, catalogApiUrl.replace(/\/$/, ""));
}

async function catalogJson<T>(url: URL): Promise<T> {
  const response = await fetch(url, { cache: "no-store" });
  if (!response.ok) {
    throw new Error(`Catalog request failed with status ${response.status}`);
  }

  return response.json() as Promise<T>;
}
