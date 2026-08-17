import { redirect } from "next/navigation";
import { readSessionUser } from "@/lib/server/auth-session";
import { AdminCatalogWorkspace } from "./workspace";

export default async function AdminCatalogPage() {
  const user = await readSessionUser();
  if (!user) {
    redirect("/login?next=/admin/catalog");
  }

  if (!user.roles.includes("Admin")) {
    redirect("/account");
  }

  return <AdminCatalogWorkspace userName={user.displayName} />;
}
