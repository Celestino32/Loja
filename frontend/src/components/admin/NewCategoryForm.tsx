"use client";

import { useState, useTransition } from "react";
import { slugify } from "@/lib/slug";
import { createCategory } from "@/app/admin/actions";

export function NewCategoryForm() {
  const [name, setName] = useState("");
  const [slug, setSlug] = useState("");
  const [slugTouched, setSlugTouched] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  function handleSubmit(formData: FormData) {
    setError(null);
    startTransition(async () => {
      try {
        await createCategory(formData);
        setName("");
        setSlug("");
        setSlugTouched(false);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Erro ao criar categoria.");
      }
    });
  }

  return (
    <form action={handleSubmit} className="flex flex-wrap items-end gap-3">
      <div>
        <label className="text-xs font-medium text-zinc-500">Nome</label>
        <input
          name="name"
          required
          value={name}
          onChange={(e) => {
            setName(e.target.value);
            if (!slugTouched) setSlug(slugify(e.target.value));
          }}
          className="mt-1 block rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>
      <div>
        <label className="text-xs font-medium text-zinc-500">Slug</label>
        <input
          name="slug"
          required
          value={slug}
          onChange={(e) => {
            setSlugTouched(true);
            setSlug(e.target.value);
          }}
          className="mt-1 block rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
        />
      </div>
      <div className="flex-1">
        <label className="text-xs font-medium text-zinc-500">Descrição (opcional)</label>
        <input
          name="description"
          className="mt-1 block w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
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
