import Link from "next/link";

export default function HomePage() {
  return (
    <main className="home-shell">
      <section className="hero-card">
        <p className="eyebrow">Phase 02 Catalog</p>
        <h1>Identity is online. Catalog shelves are opening.</h1>
        <p>
          Register, sign in, inspect your profile, and browse live products from the Catalog
          Service with search, filters, sorting, and pagination.
        </p>
        <div className="button-row">
          <Link className="button primary" href="/products">Browse products</Link>
          <Link className="button primary" href="/register">Create account</Link>
          <Link className="button secondary" href="/login">Sign in</Link>
        </div>
      </section>
    </main>
  );
}
