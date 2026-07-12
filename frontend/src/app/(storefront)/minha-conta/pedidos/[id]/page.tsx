import Link from "next/link";
import { notFound, redirect } from "next/navigation";
import { getAuthToken, getServerSession } from "@/lib/auth";
import { ApiError, serverApiFetch } from "@/lib/api";
import { formatDateTime, formatKwanza, formatOrderStatus, formatPaymentMethod } from "@/lib/format";
import type { OrderDto } from "@/lib/types";
import { ReviewForm } from "@/components/storefront/ReviewForm";

interface PedidoDetailPageProps {
  params: Promise<{ id: string }>;
}

export default async function PedidoDetailPage({ params }: PedidoDetailPageProps) {
  const { id } = await params;

  const session = await getServerSession();
  if (!session) redirect(`/login?redirectTo=/minha-conta/pedidos/${id}`);

  const token = await getAuthToken();

  let order: OrderDto;
  try {
    order = await serverApiFetch<OrderDto>(`/api/orders/${id}`, { token });
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) notFound();
    throw error;
  }

  const reviewedProductIds =
    order.status === "Concluido"
      ? await serverApiFetch<string[]>(`/api/reviews/mine?orderId=${id}`, { token })
      : [];

  return (
    <div className="mx-auto max-w-2xl px-4 py-10 sm:px-6">
      <Link href="/minha-conta/pedidos" className="text-sm text-blue-600 hover:underline">
        ← Meus pedidos
      </Link>

      <div className="mt-2 flex items-center justify-between">
        <h1 className="text-2xl font-bold text-zinc-900 dark:text-white">{order.orderNumber}</h1>
        <span className="rounded-full bg-zinc-100 px-3 py-1 text-sm font-medium text-zinc-700 dark:bg-zinc-800 dark:text-zinc-200">
          {formatOrderStatus(order.status)}
        </span>
      </div>
      <p className="text-sm text-zinc-500">{formatDateTime(order.createdAtUtc)}</p>

      <section className="mt-6 rounded-lg border border-zinc-200 dark:border-zinc-800">
        <div className="border-b border-zinc-200 px-4 py-3 dark:border-zinc-800">
          <h2 className="font-semibold text-zinc-900 dark:text-white">Itens</h2>
        </div>
        <ul className="divide-y divide-zinc-100 dark:divide-zinc-900">
          {order.items.map((item) => (
            <li key={item.productId} className="flex justify-between px-4 py-3 text-sm">
              <span className="text-zinc-600 dark:text-zinc-300">
                {item.quantity}× {item.productName}
              </span>
              <span className="font-medium text-zinc-900 dark:text-zinc-100">{formatKwanza(item.lineTotal)}</span>
            </li>
          ))}
        </ul>
        <div className="space-y-1 border-t border-zinc-200 px-4 py-3 text-sm dark:border-zinc-800">
          <div className="flex justify-between text-zinc-500">
            <span>Frete</span>
            <span>{formatKwanza(order.shippingCost)}</span>
          </div>
          <div className="flex justify-between text-base font-bold text-zinc-900 dark:text-white">
            <span>Total</span>
            <span>{formatKwanza(order.total)}</span>
          </div>
        </div>
      </section>

      {order.fulfillmentType === "Entrega" && (
        <section className="mt-6 rounded-lg border border-zinc-200 p-4 dark:border-zinc-800">
          <h2 className="font-semibold text-zinc-900 dark:text-white">Entrega</h2>
          <p className="mt-2 text-sm text-zinc-600 dark:text-zinc-300">
            {order.shippingStreet}, {order.shippingMunicipality} — {order.shippingProvince}
          </p>
          {order.shippingReference && (
            <p className="text-sm text-zinc-500">Referência: {order.shippingReference}</p>
          )}
        </section>
      )}

      {order.payment && (
        <section className="mt-6 rounded-lg border border-zinc-200 p-4 dark:border-zinc-800">
          <h2 className="font-semibold text-zinc-900 dark:text-white">Pagamento</h2>
          <p className="mt-1 text-sm text-zinc-600 dark:text-zinc-300">
            {formatPaymentMethod(order.payment.method)} ·{" "}
            <span className={order.payment.status === "Confirmado" ? "text-emerald-600" : "text-amber-600"}>
              {order.payment.status === "Confirmado" ? "Confirmado" : "Aguardando confirmação"}
            </span>
          </p>
          {order.payment.status !== "Confirmado" && order.payment.instructions && (
            <p className="mt-2 rounded-md bg-amber-50 p-3 text-sm text-amber-800 dark:bg-amber-950 dark:text-amber-200">
              {order.payment.instructions}
            </p>
          )}
        </section>
      )}

      {order.status === "Concluido" && (
        <section className="mt-6 rounded-lg border border-zinc-200 p-4 dark:border-zinc-800">
          <h2 className="font-semibold text-zinc-900 dark:text-white">Avaliações</h2>
          <div className="mt-3 space-y-3">
            {order.items.map((item) =>
              reviewedProductIds.includes(item.productId) ? (
                <p key={item.productId} className="text-sm text-zinc-500">
                  ✓ Você já avaliou {item.productName}
                </p>
              ) : (
                <ReviewForm
                  key={item.productId}
                  productId={item.productId}
                  orderId={order.id}
                  productName={item.productName}
                />
              ),
            )}
          </div>
        </section>
      )}

      {order.hasInvoice && (
        <section className="mt-6 rounded-lg border border-zinc-200 p-4 dark:border-zinc-800">
          <h2 className="font-semibold text-zinc-900 dark:text-white">Fatura</h2>
          <div className="mt-2 flex gap-4 text-sm">
            <a href={`/api/invoices/${order.id}`} target="_blank" className="text-blue-600 hover:underline">
              Ver fatura
            </a>
            <a
              href={`/api/invoices/${order.id}?segundaVia=true`}
              target="_blank"
              className="text-blue-600 hover:underline"
            >
              Baixar 2ª via
            </a>
          </div>
        </section>
      )}
    </div>
  );
}
