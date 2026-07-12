"use client";

import { useState, useTransition } from "react";
import { useRouter } from "next/navigation";
import { slugify } from "@/lib/slug";
import { createProduct, updateProduct } from "@/app/admin/actions";
import type { CategoryDto, ProductDto } from "@/lib/types";

export function ProductForm({
  categories,
  product,
}: {
  categories: CategoryDto[];
  product?: ProductDto;
}) {
  const router = useRouter();
  const isEditing = !!product;

  const [name, setName] = useState(product?.name ?? "");
  const [slug, setSlug] = useState(product?.slug ?? "");
  const [slugTouched, setSlugTouched] = useState(isEditing);
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  function handleNameChange(value: string) {
    setName(value);
    if (!slugTouched) setSlug(slugify(value));
  }

  function handleSubmit(formData: FormData) {
    setError(null);
    startTransition(async () => {
      try {
        if (isEditing) {
          await updateProduct(product.id, formData);
          router.push("/admin/produtos");
        } else {
          await createProduct(formData);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : "Erro ao guardar produto.");
      }
    });
  }

  return (
    <form action={handleSubmit} className="mt-6 grid max-w-2xl gap-4">
      {!isEditing && (
        <div className="grid grid-cols-2 gap-4">
          <Field label="SKU">
            <input name="sku" required className={inputClass} />
          </Field>
          <Field label="Stock inicial">
            <input name="stockQuantity" type="number" min={0} defaultValue={0} required className={inputClass} />
          </Field>
        </div>
      )}

      <Field label="Nome">
        <input
          name="name"
          required
          value={name}
          onChange={(e) => handleNameChange(e.target.value)}
          className={inputClass}
        />
      </Field>

      <Field label="Slug (URL)">
        <input
          name="slug"
          required
          value={slug}
          onChange={(e) => {
            setSlugTouched(true);
            setSlug(e.target.value);
          }}
          className={inputClass}
        />
      </Field>

      <div className="grid grid-cols-2 gap-4">
        <Field label="Marca">
          <input name="brand" required defaultValue={product?.brand} className={inputClass} />
        </Field>
        <Field label="Preço (Kz)">
          <input
            name="price"
            type="number"
            min={0}
            step="0.01"
            required
            defaultValue={product?.price}
            className={inputClass}
          />
        </Field>
      </div>

      <Field label="Categoria">
        <select name="categoryId" required defaultValue={product?.categoryId} className={inputClass}>
          <option value="" disabled>
            Selecione...
          </option>
          {categories.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </select>
      </Field>

      <Field label="Descrição">
        <textarea
          name="description"
          rows={4}
          defaultValue={product?.description ?? ""}
          className={inputClass}
        />
      </Field>

      {!isEditing && (
        <fieldset className="rounded-md border border-zinc-200 p-3 dark:border-zinc-700">
          <legend className="px-1 text-xs font-medium text-zinc-500">
            Especificações técnicas (opcional)
          </legend>
          {[0, 1, 2].map((i) => (
            <div key={i} className="mt-2 grid grid-cols-2 gap-2">
              <input
                name="attributeKey"
                placeholder="Ex: RAM"
                className={inputClass}
              />
              <input
                name="attributeValue"
                placeholder="Ex: 8GB"
                className={inputClass}
              />
            </div>
          ))}
        </fieldset>
      )}
      {!isEditing && (
        <p className="text-xs text-zinc-400">
          Depois de criar o produto, você poderá enviar fotos na página de edição.
        </p>
      )}

      {error && <p className="text-sm text-red-600">{error}</p>}

      <button
        type="submit"
        disabled={isPending}
        className="mt-2 w-fit rounded-md bg-blue-600 px-6 py-2 text-sm font-semibold text-white hover:bg-blue-500 disabled:bg-zinc-300"
      >
        {isPending ? "A guardar..." : isEditing ? "Guardar alterações" : "Criar produto"}
      </button>
    </form>
  );
}

const inputClass =
  "mt-1 w-full rounded-md border border-zinc-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none dark:border-zinc-700 dark:bg-zinc-950";

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <label className="block text-sm font-medium text-zinc-700 dark:text-zinc-200">
      {label}
      {children}
    </label>
  );
}
