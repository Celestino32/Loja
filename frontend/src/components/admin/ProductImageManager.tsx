"use client";

import { useRef, useState, useTransition } from "react";
import Image from "next/image";
import { useRouter } from "next/navigation";
import { removeProductImage, uploadProductImage } from "@/app/admin/actions";
import { resolveMediaUrl } from "@/lib/media";
import type { ProductImageDto } from "@/lib/types";

export function ProductImageManager({ productId, images }: { productId: string; images: ProductImageDto[] }) {
  const router = useRouter();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();
  const [removingId, setRemovingId] = useState<string | null>(null);

  function handleFileChange(event: React.ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0];
    if (!file) return;

    setError(null);
    const formData = new FormData();
    formData.set("file", file);

    startTransition(async () => {
      try {
        await uploadProductImage(productId, formData);
        router.refresh();
      } catch (err) {
        setError(err instanceof Error ? err.message : "Erro ao enviar imagem.");
      } finally {
        if (fileInputRef.current) fileInputRef.current.value = "";
      }
    });
  }

  function handleRemove(imageId: string) {
    setError(null);
    setRemovingId(imageId);
    startTransition(async () => {
      try {
        await removeProductImage(productId, imageId);
        router.refresh();
      } catch (err) {
        setError(err instanceof Error ? err.message : "Erro ao remover imagem.");
      } finally {
        setRemovingId(null);
      }
    });
  }

  return (
    <div>
      <div className="flex flex-wrap gap-3">
        {images.map((image) => (
          <div key={image.id} className="group relative h-24 w-24 overflow-hidden rounded-md border border-zinc-200 dark:border-zinc-700">
            <Image src={resolveMediaUrl(image.url)!} alt="" fill sizes="96px" className="object-cover" unoptimized />
            <button
              type="button"
              onClick={() => handleRemove(image.id)}
              disabled={isPending && removingId === image.id}
              className="absolute right-1 top-1 flex h-6 w-6 items-center justify-center rounded-full bg-red-600 text-xs font-bold text-white opacity-0 transition-opacity group-hover:opacity-100 disabled:opacity-100"
              aria-label="Remover imagem"
            >
              ×
            </button>
          </div>
        ))}

        <label className="flex h-24 w-24 cursor-pointer flex-col items-center justify-center rounded-md border border-dashed border-zinc-300 text-center text-xs text-zinc-500 hover:border-blue-400 hover:text-blue-600 dark:border-zinc-700">
          {isPending && removingId === null ? "Enviando..." : "+ Adicionar foto"}
          <input
            ref={fileInputRef}
            type="file"
            accept="image/jpeg,image/png,image/webp"
            className="hidden"
            onChange={handleFileChange}
            disabled={isPending}
          />
        </label>
      </div>

      {error && <p className="mt-2 text-sm text-red-600">{error}</p>}
      <p className="mt-2 text-xs text-zinc-400">JPEG, PNG ou WebP, até 5MB.</p>
    </div>
  );
}
