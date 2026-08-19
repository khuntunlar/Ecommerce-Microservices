import Link from "next/link";
import { notFound } from "next/navigation";
import { getProductDetail } from "@/lib/server/catalog-api";

type ProductDetailPageProps = {
  params: Promise<{ id: string }>;
};

export default async function ProductDetailPage({ params }: ProductDetailPageProps) {
  const { id } = await params;
  let detail;

  try {
    detail = await getProductDetail(id);
  } catch {
    notFound();
  }

  const { product, images } = detail;
  const primaryImage = images.find((image) => image.isPrimary) ?? images[0];
  const initials = product.name.slice(0, 2).toUpperCase();

  return (
    <main className="product-detail-shell">
      <Link className="back-link" href="/products">Back to products</Link>

      <section className="product-detail-card">
        <div className="product-detail-media">
          {primaryImage ? (
            <img src={primaryImage.url} alt={primaryImage.altText} />
          ) : (
            <div className="product-detail-fallback" aria-hidden="true"><span>{initials}</span></div>
          )}
        </div>

        <div className="product-detail-copy">
          <p className="product-sku">{product.sku}</p>
          <h1>{product.name}</h1>
          <p>{product.description}</p>
          <div className="product-detail-price-row">
            <strong>${product.price.toFixed(2)}</strong>
            <span>{product.isActive ? "Available" : "Hidden"}</span>
          </div>
          <div className="product-detail-meta">
            <span>Category</span><code>{product.categoryId}</code>
            <span>Brand</span><code>{product.brandId}</code>
          </div>
        </div>
      </section>

      <section className="image-gallery" aria-label="Product images">
        <div>
          <p className="eyebrow">Gallery</p>
          <h2>{images.length ? `${images.length} image${images.length === 1 ? "" : "s"}` : "No images yet"}</h2>
        </div>
        {images.length > 0 ? (
          <div className="image-gallery-grid">
            {images.map((image) => (
              <article className={image.isPrimary ? "gallery-card primary-image" : "gallery-card"} key={image.id}>
                <img src={image.url} alt={image.altText} />
                <div>
                  <strong>{image.altText}</strong>
                  <span>{image.isPrimary ? "Primary" : `Sort ${image.sortOrder}`}</span>
                </div>
              </article>
            ))}
          </div>
        ) : (
          <p className="gallery-empty">Admin users can add product images from the Catalog API. This page is ready to display them.</p>
        )}
      </section>
    </main>
  );
}
