import type { Metadata } from "next";
import Link from "next/link";
import "./globals.css";

export const metadata: Metadata = {
  title: "Tun Shop Catalog",
  description: "Authentication and catalog frontend for the ecommerce microservices platform"
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="en">
      <body>
        <nav className="topbar" aria-label="Main navigation">
          <Link className="logo" href="/">Ecommerce</Link>
          <div>
            <Link href="/products">Products</Link>
            <Link href="/login">Login</Link>
            <Link href="/register">Register</Link>
            <Link href="/account">Account</Link>
            <Link href="/admin/catalog">Admin</Link>
          </div>
        </nav>
        {children}
      </body>
    </html>
  );
}
