import { NextRequest } from "next/server";
import { catalogAdminFetch, proxyJsonBody } from "@/lib/server/catalog-admin-api";

type RouteContext = { params: Promise<{ id: string }> };
export async function PUT(request: NextRequest, context: RouteContext) { const { id } = await context.params; return catalogAdminFetch(`/api/v1/products/${id}`, { method: "PUT", body: await proxyJsonBody(request) }); }
export async function DELETE(_request: NextRequest, context: RouteContext) { const { id } = await context.params; return catalogAdminFetch(`/api/v1/products/${id}`, { method: "DELETE" }); }
