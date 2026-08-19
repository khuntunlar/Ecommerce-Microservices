import { NextRequest } from "next/server";
import { catalogAdminFetch, proxyJsonBody } from "@/lib/server/catalog-admin-api";

export function GET() { return catalogAdminFetch("/api/v1/categories"); }
export async function POST(request: NextRequest) { return catalogAdminFetch("/api/v1/categories", { method: "POST", body: await proxyJsonBody(request) }); }
