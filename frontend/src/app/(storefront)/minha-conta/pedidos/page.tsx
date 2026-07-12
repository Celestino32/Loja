import Link from "next/link";
import { redirect } from "next/navigation";
import { getAuthToken, getServerSession } from "@/lib/auth";
import { serverApiFetch } from "@/lib/api";
import { formatDateTime, formatKwanza, formatOrderStatus } from "@/lib/format";
import type { OrderSummaryDto } from "@/lib/types";

export default async function MeusPedidosPage() {
  const session = await getServerSession();
  if (!session) redirect("/login?redirectTo=/minha-conta/pedidos");

  const token = await getAuthToken();
  const orders = await serverApiFetch<OrderSummaryDto[]>("/api/orders/mine", { token });

  return (
    <div className="mx-auto max-w-3xl px-4 py-10 sm:px-6">
      <h1 className="text-2xl font-bold text-zinc-900 dark:text-white">Meus pedidos</h1>

      {orders.length === 0 ? (
        <p className="mt-6 text-sm text-zinc-500">
          Ainda não fez nenhum pedido.{" "}
          <Link href="/produtos" className="text-blue-600 hover:underline">
            Ver produtos
          </Link>
        </p>
      ) : (
        <div className="mt-6 divide-y divide-zinc-200 rounded-lg border border-zinc-200 dark:divide-zinc-800 dark:border-zinc-800">
          {orders.map((order) => (
            <Link
              key={order.id}
              href={`/minha-conta/pedidos/${order.id}`}
              className="flex items-center justify-between p-4 hover:bg-zinc-50 dark:hover:bg-zinc-900"
            >
              <div>
                <p className="font-medium text-zinc-900 dark:text-zinc-100">{order.orderNumber}</p>
                <p className="text-sm text-zinc-500">{formatDateTime(order.createdAtUtc)}</p>
              </div>
              <div className="text-right">
                <p className="font-medium text-zinc-900 dark:text-zinc-100">{formatKwanza(order.total)}</p>
                <p className="text-sm text-zinc-500">{formatOrderStatus(order.status)}</p>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
