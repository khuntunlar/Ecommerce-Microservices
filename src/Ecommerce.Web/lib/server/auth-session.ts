import { cookies } from "next/headers";
import { NextResponse } from "next/server";
import type { AuthSession } from "@/lib/auth-api";

const accessTokenCookie = "ecommerce_access_token";
const refreshTokenCookie = "ecommerce_refresh_token";
const userCookie = "ecommerce_user";
const cookieOptions = {
  httpOnly: true,
  sameSite: "lax" as const,
  secure: process.env.NODE_ENV === "production",
  path: "/"
};

export async function readAccessToken(): Promise<string | undefined> {
  return (await cookies()).get(accessTokenCookie)?.value;
}

export async function readRefreshToken(): Promise<string | undefined> {
  return (await cookies()).get(refreshTokenCookie)?.value;
}

export async function readSessionUser(): Promise<AuthSession["user"] | undefined> {
  const value = (await cookies()).get(userCookie)?.value;
  if (!value) {
    return undefined;
  }

  try {
    return JSON.parse(value) as AuthSession["user"];
  } catch {
    return undefined;
  }
}

export async function setSessionCookies(response: NextResponse, session: AuthSession): Promise<void> {
  response.cookies.set(accessTokenCookie, session.accessToken, {
    ...cookieOptions,
    maxAge: session.expiresIn
  });
  response.cookies.set(refreshTokenCookie, session.refreshToken, {
    ...cookieOptions,
    maxAge: 60 * 60 * 24 * 7
  });
  response.cookies.set(userCookie, JSON.stringify(session.user), {
    ...cookieOptions,
    maxAge: 60 * 60 * 24 * 7
  });
}

export function clearSessionCookies(response: NextResponse): void {
  response.cookies.set(accessTokenCookie, "", { ...cookieOptions, maxAge: 0 });
  response.cookies.set(refreshTokenCookie, "", { ...cookieOptions, maxAge: 0 });
  response.cookies.set(userCookie, "", { ...cookieOptions, maxAge: 0 });
}
