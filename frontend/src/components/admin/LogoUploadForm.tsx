"use client";

import { useRef, useState, useTransition } from "react";
import { useRouter } from "next/navigation";
import { uploadStoreLogo } from "@/app/admin/actions";
import { resolveMediaUrl } from "@/lib/media";

export function LogoUploadForm({ storeName, logoUrl }: { storeName: string; logoUrl: string | null }) {
  const router = useRouter();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [isPending, startTransition] = useTransition();

  const resolvedLogoUrl = resolveMediaUrl(logoUrl);
  const displayUrl = preview ?? resolvedLogoUrl;

  function handleFileChange(event: React.ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0];
    if (!file) return;

    setError(null);
    setSuccess(false);
    setPreview(URL.createObjectURL(file));

    const formData = new FormData();
    formData.set("file", file);

    startTransition(async () => {
      try {
        await uploadStoreLogo(formData);
        setSuccess(true);
        router.refresh();
      } catch (err) {
        setError(err instanceof Error ? err.message : "Erro ao enviar o logotipo.");
        setPreview(null);
      } finally {
        if (fileInputRef.current) fileInputRef.current.value = "";
      }
    });
  }

  return (
    <div>
      <div className="flex items-center gap-6">
        <div className="flex h-24 w-48 items-center justify-center rounded-md border border-zinc-200 bg-white p-2 dark:border-zinc-700">
          {displayUrl ? (
            // eslint-disable-next-line @next/next/no-img-element
            <img src={displayUrl} alt={storeName} className="max-h-full max-w-full object-contain" />
          ) : (
            <span className="text-sm text-zinc-400">{storeName}</span>
          )}
        </div>

        <label className="cursor-pointer rounded-md border border-zinc-300 px-4 py-2 text-sm font-medium text-zinc-700 hover:border-zinc-400 dark:border-zinc-700 dark:text-zinc-200">
          {isPending ? "Enviando..." : "Enviar novo logotipo"}
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

      {error && <p className="mt-3 text-sm text-red-600">{error}</p>}
      {success && <p className="mt-3 text-sm text-emerald-600">Logotipo atualizado com sucesso.</p>}
      <p className="mt-3 text-xs text-zinc-400">JPEG, PNG ou WebP, até 5MB. Substitui o logotipo atual em toda a loja.</p>
    </div>
  );
}
