import { NextResponse } from "next/server";
import { cookies } from "next/headers";
import { ApiError, serverApiFetch } from "@/lib/api";
import { AUTH_COOKIE_NAME } from "@/lib/auth";

interface AuthResponse {
  token: string;
  expiresAtUtc: string;
  fullName: string;
  email: string;
  roles: string[];
}

export async function POST(request: Request) {
  const body = await request.json();

  try {
    const result = await serverApiFetch<AuthResponse>("/api/auth/register", {
      method: "POST",
      body: JSON.stringify(body),
    });

    const store = await cookies();
    store.set(AUTH_COOKIE_NAME, result.token, {
      httpOnly: true,
      sameSite: "lax",
      secure: process.env.NODE_ENV === "production",
      expires: new Date(result.expiresAtUtc),
      path: "/",
    });

    return NextResponse.json({
      fullName: result.fullName,
      email: result.email,
      roles: result.roles,
    });
  } catch (error) {
    if (error instanceof ApiError) {
      return NextResponse.json({ message: error.message }, { status: error.status });
    }
    return NextResponse.json({ message: "Erro ao criar conta." }, { status: 500 });
  }
}
