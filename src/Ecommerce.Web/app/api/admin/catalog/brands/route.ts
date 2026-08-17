import { NextRequest } from "next/server";
import { catalogAdminFetch, proxyJsonBody } from "@/lib/server/catalog-admin-api";

export function GET() { return catalogAdminFetch("/api/v1/brands"); }
export async function POST(request: NextRequest) { return catalogAdminFetch("/api/v1/brands", { method: "POST", body: await proxyJsonBody(request) }); }
