import { NextResponse } from "next/server";
import { ApiError, serverApiFetch } from "@/lib/api";
import { getAuthToken } from "@/lib/auth";
import type { CheckoutRequest, CheckoutResultDto } from "@/lib/types";

export async function POST(request: Request) {
  const token = await getAuthToken();
  if (!token) {
    return NextResponse.json({ message: "Sessão expirada. Entre novamente." }, { status: 401 });
  }

  const body = (await request.json()) as CheckoutRequest;

  try {
    const result = await serverApiFetch<CheckoutResultDto>("/api/orders/checkout", {
      method: "POST",
      token,
      body: JSON.stringify(body),
    });

    return NextResponse.json(result);
  } catch (error) {
    if (error instanceof ApiError) {
      return NextResponse.json({ message: error.message }, { status: error.status });
    }
    return NextResponse.json({ message: "Erro ao finalizar a compra." }, { status: 500 });
  }
}
