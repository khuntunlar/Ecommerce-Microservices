import { NextRequest } from "next/server";
import { catalogAdminFetch } from "@/lib/server/catalog-admin-api";

type RouteContext = { params: Promise<{ id: string; imageId: string }> };
export async function DELETE(_request: NextRequest, context: RouteContext) { const { id, imageId } = await context.params; return catalogAdminFetch(`/api/v1/products/${id}/images/${imageId}`, { method: "DELETE" }); }
