"use client";

import { useState, useTransition } from "react";
import { useRouter } from "next/navigation";
import { adjustStock } from "@/app/admin/actions";

export function StockAdjuster({ productId, stockQuantity }: { productId: string; stockQuantity: number }) {
  const router = useRouter();
  const [delta, setDelta] = useState(0);
  const [isPending, startTransition] = useTransition();

  function apply(quantity: number) {
    if (quantity === 0) return;
    startTransition(async () => {
      await adjustStock(productId, quantity);
      setDelta(0);
      router.refresh();
    });
  }

  return (
    <div className="flex items-center gap-3">
      <span className="text-2xl font-bold text-zinc-900 dark:text-white">{stockQuantity}</span>
      <span className="text-sm text-zinc-500">unidades em stock</span>
      <div className="ml-4 flex items-center gap-2">
        <input
          type="number"
          value={delta}
          onChange={(e) => setDelta(Number(e.target.value))}
          className="w-20 rounded-md border border-zinc-300 px-2 py-1 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
        <button
          type="button"
          disabled={isPending || delta === 0}
          onClick={() => apply(delta)}
          className="rounded-md border border-zinc-300 px-3 py-1 text-sm font-medium text-zinc-700 hover:border-zinc-400 disabled:opacity-50 dark:border-zinc-700 dark:text-zinc-200"
        >
          Ajustar
        </button>
      </div>
    </div>
  );
}
