import { cookies } from "next/headers";
import type { AuthSession, StaffRole } from "./types";

export const AUTH_COOKIE_NAME = "lv_token";

interface JwtPayload {
  sub: string;
  email: string;
  name: string;
  role?: StaffRole | StaffRole[];
  exp: number;
}

function decodeJwtPayload(token: string): JwtPayload | null {
  try {
    const [, payload] = token.split(".");
    const normalized = payload.replace(/-/g, "+").replace(/_/g, "/");
    const json = Buffer.from(normalized, "base64").toString("utf-8");
    return JSON.parse(json) as JwtPayload;
  } catch {
    return null;
  }
}

/** Lê a sessão a partir do cookie httpOnly. Não valida a assinatura — a validação real ocorre na API. */
export async function getServerSession(): Promise<AuthSession | null> {
  const store = await cookies();
  const token = store.get(AUTH_COOKIE_NAME)?.value;
  if (!token) return null;

  const payload = decodeJwtPayload(token);
  if (!payload) return null;

  if (payload.exp * 1000 < Date.now()) return null;

  const roles = payload.role ? (Array.isArray(payload.role) ? payload.role : [payload.role]) : [];

  return { fullName: payload.name, email: payload.email, roles };
}

export async function getAuthToken(): Promise<string | undefined> {
  const store = await cookies();
  return store.get(AUTH_COOKIE_NAME)?.value;
}

export function hasStaffRole(session: AuthSession | null): boolean {
  return !!session && session.roles.length > 0;
}
