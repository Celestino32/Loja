import { NextResponse } from "next/server";
import { getAuthToken } from "@/lib/auth";

const INTERNAL_API_URL = process.env.API_INTERNAL_URL ?? process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5050";

export async function GET(request: Request, { params }: { params: Promise<{ orderId: string }> }) {
  const { orderId } = await params;
  const token = await getAuthToken();
  if (!token) {
    return NextResponse.json({ message: "Sessão expirada. Entre novamente." }, { status: 401 });
  }

  const { searchParams } = new URL(request.url);
  const segundaVia = searchParams.get("segundaVia") === "true";

  const response = await fetch(
    `${INTERNAL_API_URL}/api/invoices/order/${orderId}/pdf?segundaVia=${segundaVia}`,
    { headers: { Authorization: `Bearer ${token}` }, cache: "no-store" },
  );

  if (!response.ok) {
    return NextResponse.json({ message: "Não foi possível obter a fatura." }, { status: response.status });
  }

  const buffer = await response.arrayBuffer();
  const fileName = response.headers.get("content-disposition")?.match(/filename="?([^"]+)"?/)?.[1] ?? "fatura.pdf";

  return new NextResponse(buffer, {
    headers: {
      "Content-Type": "application/pdf",
      "Content-Disposition": `inline; filename="${fileName}"`,
    },
  });
}
