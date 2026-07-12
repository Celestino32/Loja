import Link from "next/link";
import Form from "next/form";
import { getAuthToken } from "@/lib/auth";
import { serverApiFetch } from "@/lib/api";
import { formatDateTime, formatKwanza, formatOrderStatus } from "@/lib/format";
import type { OrderStatus, PagedResult, OrderSummaryDto } from "@/lib/types";

interface AdminPedidosPageProps {
  searchParams: Promise<{ search?: string; status?: string; page?: string }>;
}

const statuses: OrderStatus[] = [
  "Pendente",
  "Pago",
  "Preparando",
  "ProntoParaEntregaOuRetirada",
  "Concluido",
  "Cancelado",
];

export default async function AdminPedidosPage({ searchParams }: AdminPedidosPageProps) {
  const resolved = await searchParams;
  const search = resolved.search ?? "";
  const status = resolved.status ?? "";
  const page = Math.max(1, Number(resolved.page ?? "1") || 1);

  const token = await getAuthToken();
  const params = new URLSearchParams({ page: String(page), pageSize: "20" });
  if (search) params.set("search", search);
  if (status) params.set("status", status);

  const result = await serverApiFetch<PagedResult<OrderSummaryDto>>(`/api/orders?${params.toString()}`, { token });

  return (
    <div>
      <h1 className="text-xl font-bold text-zinc-900 dark:text-white">Pedidos</h1>

      <div className="mt-4 flex flex-wrap items-center gap-4">
        <Form action="/admin/pedidos" className="flex gap-2">
          {status && <input type="hidden" name="status" value={status} />}
          <input
            type="text"
            name="search"
            defaultValue={search}
            placeholder="Pesquisar por nº do pedido..."
            className="w-64 rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
          />
          <button
            type="submit"
            className="rounded-md border border-zinc-300 px-4 py-2 text-sm font-medium text-zinc-700 hover:border-zinc-400 dark:border-zinc-700 dark:text-zinc-200"
          >
            Buscar
          </button>
        </Form>

        <div className="flex flex-wrap gap-2">
          <Link
            href="/admin/pedidos"
            className={`rounded-full px-3 py-1 text-xs font-medium ${
              !status ? "bg-blue-600 text-white" : "bg-zinc-100 text-zinc-600 dark:bg-zinc-800 dark:text-zinc-300"
            }`}
          >
            Todos
          </Link>
          {statuses.map((s) => (
            <Link
              key={s}
              href={`/admin/pedidos?status=${s}`}
              className={`rounded-full px-3 py-1 text-xs font-medium ${
                status === s ? "bg-blue-600 text-white" : "bg-zinc-100 text-zinc-600 dark:bg-zinc-800 dark:text-zinc-300"
              }`}
            >
              {formatOrderStatus(s)}
            </Link>
          ))}
        </div>
      </div>

      <div className="mt-4 overflow-x-auto rounded-lg border border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-zinc-200 text-left text-xs uppercase tracking-wide text-zinc-400 dark:border-zinc-800">
              <th className="px-4 py-3">Pedido</th>
              <th className="px-4 py-3">Data</th>
              <th className="px-4 py-3">Total</th>
              <th className="px-4 py-3">Estado</th>
              <th className="px-4 py-3" />
            </tr>
          </thead>
          <tbody>
            {result.items.map((order) => (
              <tr key={order.id} className="border-b border-zinc-100 last:border-0 dark:border-zinc-900">
                <td className="px-4 py-3 font-medium text-zinc-900 dark:text-zinc-100">{order.orderNumber}</td>
                <td className="px-4 py-3 text-zinc-500">{formatDateTime(order.createdAtUtc)}</td>
                <td className="px-4 py-3 text-zinc-500">{formatKwanza(order.total)}</td>
                <td className="px-4 py-3 text-zinc-500">{formatOrderStatus(order.status)}</td>
                <td className="px-4 py-3 text-right">
                  <Link href={`/admin/pedidos/${order.id}`} className="text-sm font-medium text-blue-600 hover:underline">
                    Ver
                  </Link>
                </td>
              </tr>
            ))}
            {result.items.length === 0 && (
              <tr>
                <td colSpan={5} className="px-4 py-8 text-center text-zinc-400">
                  Nenhum pedido encontrado.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {result.totalPages > 1 && (
        <p className="mt-4 text-sm text-zinc-500">
          Página {result.page} de {result.totalPages} ({result.totalCount} pedidos)
        </p>
      )}
    </div>
  );
}
