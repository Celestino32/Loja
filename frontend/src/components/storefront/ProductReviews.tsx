import { serverApiFetch } from "@/lib/api";
import { formatDateTime } from "@/lib/format";
import type { ProductReviewsDto } from "@/lib/types";
import { StarRating } from "./StarRating";

async function getReviews(productId: string): Promise<ProductReviewsDto> {
  try {
    return await serverApiFetch<ProductReviewsDto>(`/api/reviews/product/${productId}`);
  } catch {
    return { averageRating: 0, totalCount: 0, items: [] };
  }
}

export async function ProductReviews({ productId }: { productId: string }) {
  const reviews = await getReviews(productId);

  return (
    <section className="mt-12 border-t border-zinc-200 pt-8 dark:border-zinc-800">
      <div className="flex items-center gap-3">
        <h2 className="text-lg font-semibold text-zinc-900 dark:text-white">Avaliações</h2>
        {reviews.totalCount > 0 && (
          <>
            <StarRating value={reviews.averageRating} />
            <span className="text-sm text-zinc-500">
              {reviews.averageRating.toFixed(1)} · {reviews.totalCount}{" "}
              {reviews.totalCount === 1 ? "avaliação" : "avaliações"}
            </span>
          </>
        )}
      </div>

      {reviews.items.length === 0 ? (
        <p className="mt-4 text-sm text-zinc-500">
          Ainda não há avaliações para este produto. Compre e seja o primeiro a avaliar.
        </p>
      ) : (
        <ul className="mt-6 space-y-6">
          {reviews.items.map((review) => (
            <li key={review.id} className="border-b border-zinc-100 pb-6 last:border-0 dark:border-zinc-900">
              <div className="flex items-center justify-between">
                <p className="font-medium text-zinc-900 dark:text-zinc-100">{review.customerName}</p>
                <span className="text-xs text-zinc-400">{formatDateTime(review.createdAtUtc)}</span>
              </div>
              <div className="mt-1">
                <StarRating value={review.rating} size={14} />
              </div>
              {review.comment && (
                <p className="mt-2 text-sm text-zinc-600 dark:text-zinc-300">{review.comment}</p>
              )}
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
