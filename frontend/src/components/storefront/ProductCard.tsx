import Link from "next/link";
import { formatKwanza } from "@/lib/format";
import { resolveMediaUrl } from "@/lib/media";
import type { ProductListItemDto } from "@/lib/types";

export function ProductCard({ product }: { product: ProductListItemDto }) {
  const outOfStock = product.stockQuantity <= 0;
  const imageUrl = resolveMediaUrl(product.primaryImageUrl);

  return (
    <Link
      href={`/produtos/${product.slug}`}
      className="group flex flex-col overflow-hidden rounded-lg border border-zinc-200 bg-white transition hover:shadow-md dark:border-zinc-800 dark:bg-zinc-900"
    >
      <div className="flex aspect-square items-center justify-center bg-zinc-100 dark:bg-zinc-800">
        {imageUrl ? (
          // eslint-disable-next-line @next/next/no-img-element
          <img
            src={imageUrl}
            alt={product.name}
            className="h-full w-full object-cover"
          />
        ) : (
          <span className="text-sm text-zinc-400">{product.brand}</span>
        )}
      </div>

      <div className="flex flex-1 flex-col gap-1 p-4">
        <span className="text-xs font-medium uppercase tracking-wide text-blue-600">
          {product.categoryName ?? product.brand}
        </span>
        <h3 className="line-clamp-2 text-sm font-medium text-zinc-900 group-hover:text-blue-600 dark:text-zinc-100">
          {product.name}
        </h3>
        <div className="mt-auto flex items-center justify-between pt-2">
          <span className="text-base font-bold text-zinc-900 dark:text-white">
            {formatKwanza(product.price)}
          </span>
          {outOfStock && (
            <span className="text-xs font-medium text-red-600">Esgotado</span>
          )}
        </div>
      </div>
    </Link>
  );
}
