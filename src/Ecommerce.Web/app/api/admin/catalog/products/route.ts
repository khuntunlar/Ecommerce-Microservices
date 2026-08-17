import { NextRequest } from "next/server";
import { catalogAdminFetch, proxyJsonBody } from "@/lib/server/catalog-admin-api";

export async function GET(request: NextRequest) { return catalogAdminFetch(`/api/v1/products${request.nextUrl.search}`); }
export async function POST(request: NextRequest) { return catalogAdminFetch("/api/v1/products", { method: "POST", body: await proxyJsonBody(request) }); }
