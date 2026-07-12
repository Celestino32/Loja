import { NextResponse } from "next/server";
import { getAuthToken } from "@/lib/auth";

const INTERNAL_API_URL = process.env.API_INTERNAL_URL ?? process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5050";

export async function GET(request: Request) {
  const token = await getAuthToken();
  if (!token) {
    return NextResponse.json({ message: "Sessão expirada. Entre novamente." }, { status: 401 });
  }

  const { searchParams } = new URL(request.url);
  const from = searchParams.get("from");
  const to = searchParams.get("to");
  if (!from || !to) {
    return NextResponse.json({ message: "Informe o período (from/to)." }, { status: 400 });
  }

  const response = await fetch(
    `${INTERNAL_API_URL}/api/invoices/export/saft?from=${from}&to=${to}`,
    { headers: { Authorization: `Bearer ${token}` }, cache: "no-store" },
  );

  if (!response.ok) {
    return NextResponse.json({ message: "Não foi possível gerar a exportação SAF-T." }, { status: response.status });
  }

  const buffer = await response.arrayBuffer();
  const fileName = response.headers.get("content-disposition")?.match(/filename="?([^"]+)"?/)?.[1] ?? "saft.xml";

  return new NextResponse(buffer, {
    headers: {
      "Content-Type": "application/xml",
      "Content-Disposition": `attachment; filename="${fileName}"`,
    },
  });
}
