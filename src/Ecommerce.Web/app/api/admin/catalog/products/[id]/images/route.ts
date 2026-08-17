import { NextRequest } from "next/server";
import { catalogAdminFetch, proxyJsonBody } from "@/lib/server/catalog-admin-api";

type RouteContext = { params: Promise<{ id: string }> };
export async function GET(_request: NextRequest, context: RouteContext) { const { id } = await context.params; return catalogAdminFetch(`/api/v1/products/${id}/images`); }
export async function POST(request: NextRequest, context: RouteContext) { const { id } = await context.params; return catalogAdminFetch(`/api/v1/products/${id}/images`, { method: "POST", body: await proxyJsonBody(request) }); }
