"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";

export function ReviewForm({
  productId,
  orderId,
  productName,
}: {
  productId: string;
  orderId: string;
  productName: string;
}) {
  const router = useRouter();
  const [open, setOpen] = useState(false);
  const [rating, setRating] = useState(5);
  const [hoverRating, setHoverRating] = useState(0);
  const [comment, setComment] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitted, setSubmitted] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  if (submitted) {
    return <p className="text-sm text-emerald-600">Obrigado pela avaliação de {productName}!</p>;
  }

  if (!open) {
    return (
      <button type="button" onClick={() => setOpen(true)} className="text-sm font-medium text-blue-600 hover:underline">
        Avaliar {productName}
      </button>
    );
  }

  async function handleSubmit() {
    setError(null);
    setIsSubmitting(true);
    try {
      const response = await fetch("/api/reviews", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ productId, orderId, rating, comment: comment.trim() || null }),
      });

      if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.message ?? "Não foi possível enviar a avaliação.");
      }

      setSubmitted(true);
      router.refresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro inesperado.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="rounded-md border border-zinc-200 p-3 dark:border-zinc-800">
      <p className="text-sm font-medium text-zinc-900 dark:text-zinc-100">Avaliar {productName}</p>

      <div className="mt-2 flex gap-1">
        {[1, 2, 3, 4, 5].map((star) => (
          <button
            key={star}
            type="button"
            onClick={() => setRating(star)}
            onMouseEnter={() => setHoverRating(star)}
            onMouseLeave={() => setHoverRating(0)}
            aria-label={`${star} estrelas`}
            className="p-0.5"
          >
            <svg
              viewBox="0 0 20 20"
              width={22}
              height={22}
              fill={star <= (hoverRating || rating) ? "#f59e0b" : "none"}
              stroke="#f59e0b"
              strokeWidth="1.2"
            >
              <path d="M10 1.5l2.6 5.3 5.8.8-4.2 4.1 1 5.8-5.2-2.7-5.2 2.7 1-5.8L1.6 7.6l5.8-.8L10 1.5Z" />
            </svg>
          </button>
        ))}
      </div>

      <textarea
        value={comment}
        onChange={(e) => setComment(e.target.value)}
        placeholder="Conte como foi sua experiência (opcional)"
        rows={3}
        className="mt-2 w-full rounded-md border border-zinc-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none dark:border-zinc-700 dark:bg-zinc-900"
      />

      {error && <p className="mt-1 text-sm text-red-600">{error}</p>}

      <div className="mt-2 flex gap-2">
        <button
          type="button"
          onClick={handleSubmit}
          disabled={isSubmitting}
          className="rounded-md bg-blue-600 px-4 py-1.5 text-sm font-medium text-white hover:bg-blue-500 disabled:bg-zinc-300"
        >
          {isSubmitting ? "A enviar..." : "Enviar avaliação"}
        </button>
        <button
          type="button"
          onClick={() => setOpen(false)}
          className="rounded-md px-4 py-1.5 text-sm font-medium text-zinc-600 hover:bg-zinc-100 dark:text-zinc-300 dark:hover:bg-zinc-900"
        >
          Cancelar
        </button>
      </div>
    </div>
  );
}
