"use client";

import { useState, useTransition } from "react";
import { createCoupon } from "@/app/admin/actions";

export function NewCouponForm() {
  const [discountType, setDiscountType] = useState<"Percentual" | "ValorFixo">("Percentual");
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  function handleSubmit(formData: FormData) {
    setError(null);
    startTransition(async () => {
      try {
        await createCoupon(formData);
        (document.getElementById("new-coupon-form") as HTMLFormElement)?.reset();
      } catch (err) {
        setError(err instanceof Error ? err.message : "Erro ao criar cupom.");
      }
    });
  }

  return (
    <form id="new-coupon-form" action={handleSubmit} className="grid gap-3 sm:grid-cols-3">
      <div>
        <label className="text-xs font-medium text-zinc-500">Código</label>
        <input
          name="code"
          required
          placeholder="Ex: BEMVINDO10"
          className="mt-1 block w-full rounded-md border border-zinc-300 px-3 py-2 text-sm uppercase dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>

      <div>
        <label className="text-xs font-medium text-zinc-500">Tipo de desconto</label>
        <select
          name="discountType"
          value={discountType}
          onChange={(e) => setDiscountType(e.target.value as "Percentual" | "ValorFixo")}
          className="mt-1 block w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        >
          <option value="Percentual">Percentual (%)</option>
          <option value="ValorFixo">Valor fixo (Kz)</option>
        </select>
      </div>

      <div>
        <label className="text-xs font-medium text-zinc-500">
          Valor {discountType === "Percentual" ? "(%)" : "(Kz)"}
        </label>
        <input
          name="discountValue"
          type="number"
          min={0.01}
          step="0.01"
          required
          className="mt-1 block w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>

      <div>
        <label className="text-xs font-medium text-zinc-500">Pedido mínimo (Kz, opcional)</label>
        <input
          name="minOrderValue"
          type="number"
          min={0}
          step="0.01"
          className="mt-1 block w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>

      <div>
        <label className="text-xs font-medium text-zinc-500">Limite de usos (opcional)</label>
        <input
          name="maxUses"
          type="number"
          min={1}
          className="mt-1 block w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>

      <div>
        <label className="text-xs font-medium text-zinc-500">Válido até (opcional)</label>
        <input
          name="validUntil"
          type="date"
          className="mt-1 block w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>

      <div className="sm:col-span-3">
        <button
          type="submit"
          disabled={isPending}
          className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-500 disabled:bg-zinc-300"
        >
          {isPending ? "A criar..." : "Criar cupom"}
        </button>
        {error && <p className="mt-2 text-sm text-red-600">{error}</p>}
      </div>
    </form>
  );
}
