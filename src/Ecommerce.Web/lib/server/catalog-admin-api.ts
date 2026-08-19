import { NextRequest, NextResponse } from "next/server";
import { readAccessToken } from "@/lib/server/auth-session";

const catalogApiUrl = process.env.CATALOG_API_URL
  ?? process.env.NEXT_PUBLIC_CATALOG_API_URL
  ?? "http://localhost:5295";

export async function catalogAdminFetch(path: string, init: RequestInit = {}) {
  const accessToken = await readAccessToken();
  if (!accessToken) {
    return NextResponse.json({ title: "Authentication failed", detail: "Missing session." }, { status: 401 });
  }

  const response = await fetch(`${catalogApiUrl.replace(/\/$/, "")}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${accessToken}`,
      ...init.headers
    },
    cache: "no-store"
  });

  if (response.status === 204) {
    return new NextResponse(null, { status: 204 });
  }

  return NextResponse.json(await readJsonOrProblem(response), { status: response.status });
}

export async function proxyJsonBody(request: NextRequest) {
  return JSON.stringify(await request.json());
}

async function readJsonOrProblem(response: Response) {
  const text = await response.text();
  if (!text) {
    return null;
  }

  try {
    return JSON.parse(text);
  } catch {
    return { title: text };
  }
}
