import { notFound } from "next/navigation";
import { getAuthToken } from "@/lib/auth";
import { ApiError, serverApiFetch } from "@/lib/api";
import { formatDateTime, formatKwanza, formatOrderStatus, formatPaymentMethod } from "@/lib/format";
import type { OrderDto } from "@/lib/types";
import { OrderActions } from "@/components/admin/OrderActions";

interface AdminPedidoDetailPageProps {
  params: Promise<{ id: string }>;
}

export default async function AdminPedidoDetailPage({ params }: AdminPedidoDetailPageProps) {
  const { id } = await params;
  const token = await getAuthToken();

  let order: OrderDto;
  try {
    order = await serverApiFetch<OrderDto>(`/api/orders/${id}`, { token });
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) notFound();
    throw error;
  }

  return (
    <div>
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-bold text-zinc-900 dark:text-white">{order.orderNumber}</h1>
          <p className="text-sm text-zinc-500">{formatDateTime(order.createdAtUtc)}</p>
        </div>
        <span className="rounded-full bg-zinc-100 px-3 py-1 text-sm font-medium text-zinc-700 dark:bg-zinc-800 dark:text-zinc-200">
          {formatOrderStatus(order.status)}
        </span>
      </div>

      <div className="mt-4">
        <OrderActions order={order} />
      </div>

      <div className="mt-6 grid gap-6 md:grid-cols-2">
        <section className="rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
          <h2 className="font-semibold text-zinc-900 dark:text-white">Cliente</h2>
          <p className="mt-2 text-sm text-zinc-600 dark:text-zinc-300">{order.customerName}</p>
          <p className="text-sm text-zinc-500">{order.customerPhone}</p>

          {order.fulfillmentType === "Entrega" ? (
            <div className="mt-4">
              <h3 className="text-sm font-semibold text-zinc-900 dark:text-white">Entrega</h3>
              <p className="text-sm text-zinc-600 dark:text-zinc-300">
                {order.shippingStreet}, {order.shippingMunicipality} — {order.shippingProvince}
              </p>
              {order.shippingReference && (
                <p className="text-sm text-zinc-500">Referência: {order.shippingReference}</p>
              )}
            </div>
          ) : (
            <p className="mt-4 text-sm font-medium text-zinc-700 dark:text-zinc-200">Retirada na loja</p>
          )}
        </section>

        <section className="rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
          <h2 className="font-semibold text-zinc-900 dark:text-white">Pagamento</h2>
          {order.payment ? (
            <>
              <p className="mt-2 text-sm text-zinc-600 dark:text-zinc-300">
                {formatPaymentMethod(order.payment.method)}
              </p>
              <p className="text-sm">
                <span
                  className={
                    order.payment.status === "Confirmado" ? "text-emerald-600" : "text-amber-600"
                  }
                >
                  {order.payment.status === "Confirmado" ? "Confirmado" : "Aguardando confirmação"}
                </span>
                {order.payment.confirmedAtUtc && ` em ${formatDateTime(order.payment.confirmedAtUtc)}`}
              </p>
              {order.payment.externalReference && (
                <p className="text-sm text-zinc-500">Referência: {order.payment.externalReference}</p>
              )}
            </>
          ) : (
            <p className="mt-2 text-sm text-zinc-500">Sem pagamento associado.</p>
          )}

          {order.hasInvoice && (
            <div className="mt-4 flex gap-4 text-sm">
              <a href={`/api/invoices/${order.id}`} target="_blank" className="text-blue-600 hover:underline">
                Ver fatura
              </a>
              <a
                href={`/api/invoices/${order.id}?segundaVia=true`}
                target="_blank"
                className="text-blue-600 hover:underline"
              >
                2ª via
              </a>
            </div>
          )}
        </section>
      </div>

      <section className="mt-6 rounded-lg border border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
        <div className="border-b border-zinc-200 px-4 py-3 dark:border-zinc-800">
          <h2 className="font-semibold text-zinc-900 dark:text-white">Itens</h2>
        </div>
        <table className="w-full text-sm">
          <tbody>
            {order.items.map((item) => (
              <tr key={item.productId} className="border-b border-zinc-100 last:border-0 dark:border-zinc-900">
                <td className="px-4 py-3 text-zinc-900 dark:text-zinc-100">{item.productName}</td>
                <td className="px-4 py-3 text-right text-zinc-500">{item.quantity}×</td>
                <td className="px-4 py-3 text-right text-zinc-500">{formatKwanza(item.unitPrice)}</td>
                <td className="px-4 py-3 text-right font-medium text-zinc-900 dark:text-zinc-100">
                  {formatKwanza(item.lineTotal)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="space-y-1 border-t border-zinc-200 px-4 py-3 text-right text-sm dark:border-zinc-800">
          <p className="text-zinc-500">Frete: {formatKwanza(order.shippingCost)}</p>
          <p className="text-base font-bold text-zinc-900 dark:text-white">Total: {formatKwanza(order.total)}</p>
        </div>
      </section>
    </div>
  );
}
