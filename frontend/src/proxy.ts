import { NextResponse, type NextRequest } from "next/server";

const AUTH_COOKIE_NAME = "lv_token";

function decodeRoles(token: string): string[] {
  try {
    const [, payload] = token.split(".");
    const normalized = payload.replace(/-/g, "+").replace(/_/g, "/");
    const json = Buffer.from(normalized, "base64").toString("utf-8");
    const parsed = JSON.parse(json) as { role?: string | string[]; exp: number };

    if (parsed.exp * 1000 < Date.now()) return [];

    if (!parsed.role) return [];
    return Array.isArray(parsed.role) ? parsed.role : [parsed.role];
  } catch {
    return [];
  }
}

export function proxy(request: NextRequest) {
  const token = request.cookies.get(AUTH_COOKIE_NAME)?.value;
  const roles = token ? decodeRoles(token) : [];

  if (roles.length === 0) {
    const loginUrl = new URL("/login", request.url);
    loginUrl.searchParams.set("redirectTo", request.nextUrl.pathname);
    return NextResponse.redirect(loginUrl);
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/admin/:path*"],
};
