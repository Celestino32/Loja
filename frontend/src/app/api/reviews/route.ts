import { NextResponse } from "next/server";
import { ApiError, serverApiFetch } from "@/lib/api";
import { getAuthToken } from "@/lib/auth";

export async function POST(request: Request) {
  const token = await getAuthToken();
  if (!token) {
    return NextResponse.json({ message: "Sessão expirada. Entre novamente." }, { status: 401 });
  }

  const body = await request.json();

  try {
    const id = await serverApiFetch<string>("/api/reviews", {
      method: "POST",
      token,
      body: JSON.stringify(body),
    });

    return NextResponse.json({ id });
  } catch (error) {
    if (error instanceof ApiError) {
      return NextResponse.json({ message: error.message }, { status: error.status });
    }
    return NextResponse.json({ message: "Erro ao enviar avaliação." }, { status: 500 });
  }
}
