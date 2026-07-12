"use client";

import { useState, useTransition } from "react";
import { createStaff } from "@/app/admin/actions";

export function NewStaffForm() {
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [isPending, startTransition] = useTransition();

  function handleSubmit(formData: FormData) {
    setError(null);
    setSuccess(false);
    startTransition(async () => {
      try {
        await createStaff(formData);
        setSuccess(true);
        (document.getElementById("new-staff-form") as HTMLFormElement)?.reset();
      } catch (err) {
        setError(err instanceof Error ? err.message : "Erro ao criar funcionário.");
      }
    });
  }

  return (
    <form id="new-staff-form" action={handleSubmit} className="grid max-w-lg gap-4">
      <div>
        <label className="text-sm font-medium text-zinc-700 dark:text-zinc-200">Nome completo</label>
        <input
          name="fullName"
          required
          className="mt-1 w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>
      <div>
        <label className="text-sm font-medium text-zinc-700 dark:text-zinc-200">E-mail</label>
        <input
          name="email"
          type="email"
          required
          className="mt-1 w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>
      <div>
        <label className="text-sm font-medium text-zinc-700 dark:text-zinc-200">Senha temporária</label>
        <input
          name="password"
          type="password"
          required
          minLength={6}
          className="mt-1 w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>
      <div>
        <label className="text-sm font-medium text-zinc-700 dark:text-zinc-200">Papel</label>
        <select
          name="role"
          required
          defaultValue="Vendedor"
          className="mt-1 w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        >
          <option value="Vendedor">Vendedor</option>
          <option value="Gerente">Gerente</option>
          <option value="Admin">Admin</option>
        </select>
      </div>

      {error && <p className="text-sm text-red-600">{error}</p>}
      {success && <p className="text-sm text-emerald-600">Funcionário criado com sucesso.</p>}

      <button
        type="submit"
        disabled={isPending}
        className="w-fit rounded-md bg-blue-600 px-6 py-2 text-sm font-semibold text-white hover:bg-blue-500 disabled:bg-zinc-300"
      >
        {isPending ? "A criar..." : "Criar funcionário"}
      </button>
    </form>
  );
}
