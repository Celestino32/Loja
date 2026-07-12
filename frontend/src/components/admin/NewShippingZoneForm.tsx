"use client";

import { useState, useTransition } from "react";
import { createShippingZone } from "@/app/admin/actions";

export function NewShippingZoneForm() {
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  function handleSubmit(formData: FormData) {
    setError(null);
    startTransition(async () => {
      try {
        await createShippingZone(formData);
        (document.getElementById("new-zone-form") as HTMLFormElement)?.reset();
      } catch (err) {
        setError(err instanceof Error ? err.message : "Erro ao criar zona de frete.");
      }
    });
  }

  return (
    <form id="new-zone-form" action={handleSubmit} className="flex flex-wrap items-end gap-3">
      <div>
        <label className="text-xs font-medium text-zinc-500">Província</label>
        <input
          name="province"
          required
          className="mt-1 block rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>
      <div>
        <label className="text-xs font-medium text-zinc-500">Município</label>
        <input
          name="municipality"
          required
          className="mt-1 block rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>
      <div>
        <label className="text-xs font-medium text-zinc-500">Custo (Kz)</label>
        <input
          name="cost"
          type="number"
          min={0}
          step="0.01"
          required
          className="mt-1 block w-32 rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>
      <button
        type="submit"
        disabled={isPending}
        className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-500 disabled:bg-zinc-300"
      >
        {isPending ? "A criar..." : "Adicionar"}
      </button>
      {error && <p className="w-full text-sm text-red-600">{error}</p>}
    </form>
  );
}
