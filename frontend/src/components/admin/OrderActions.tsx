"use client";

import { useState, useTransition } from "react";
import {
  cancelOrder,
  completeOrder,
  confirmOrderPayment,
  markOrderReady,
  startPreparingOrder,
} from "@/app/admin/actions";
import type { OrderDto } from "@/lib/types";

export function OrderActions({ order }: { order: OrderDto }) {
  const [isPending, startTransition] = useTransition();
  const [error, setError] = useState<string | null>(null);

  function run(action: () => Promise<void>) {
    setError(null);
    startTransition(async () => {
      try {
        await action();
      } catch (err) {
        setError(err instanceof Error ? err.message : "Erro ao atualizar o pedido.");
      }
    });
  }

  const canCancel = order.status !== "Concluido" && order.status !== "Cancelado";

  return (
    <div className="flex flex-wrap items-center gap-2">
      {order.status === "Pendente" && order.payment?.status !== "Confirmado" && (
        <button
          type="button"
          disabled={isPending}
          onClick={() => run(() => confirmOrderPayment(order.id))}
          className="rounded-md bg-emerald-600 px-4 py-2 text-sm font-medium text-white hover:bg-emerald-500 disabled:opacity-50"
        >
          Confirmar pagamento
        </button>
      )}

      {order.status === "Pago" && (
        <button
          type="button"
          disabled={isPending}
          onClick={() => run(() => startPreparingOrder(order.id))}
          className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-500 disabled:opacity-50"
        >
          Iniciar preparação
        </button>
      )}

      {order.status === "Preparando" && (
        <button
          type="button"
          disabled={isPending}
          onClick={() => run(() => markOrderReady(order.id))}
          className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-500 disabled:opacity-50"
        >
          Marcar pronto
        </button>
      )}

      {order.status === "ProntoParaEntregaOuRetirada" && (
        <button
          type="button"
          disabled={isPending}
          onClick={() => run(() => completeOrder(order.id))}
          className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-500 disabled:opacity-50"
        >
          Concluir pedido
        </button>
      )}

      {canCancel && (
        <button
          type="button"
          disabled={isPending}
          onClick={() => {
            if (window.confirm("Cancelar este pedido? O estoque reservado será reposto.")) {
              run(() => cancelOrder(order.id));
            }
          }}
          className="rounded-md border border-red-300 px-4 py-2 text-sm font-medium text-red-600 hover:bg-red-50 disabled:opacity-50 dark:hover:bg-red-950"
        >
          Cancelar pedido
        </button>
      )}

      {error && <p className="w-full text-sm text-red-600">{error}</p>}
    </div>
  );
}
