"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useCart } from "@/lib/cart-context";
import { resolveMediaUrl } from "@/lib/media";
import type { ProductDto } from "@/lib/types";

export function AddToCartButton({ product }: { product: ProductDto }) {
  const { addItem } = useCart();
  const router = useRouter();
  const [quantity, setQuantity] = useState(1);
  const [added, setAdded] = useState(false);

  const inStock = product.stockQuantity > 0;

  function handleAdd() {
    addItem(
      {
        productId: product.id,
        name: product.name,
        slug: product.slug,
        price: product.price,
        imageUrl: resolveMediaUrl(product.images[0]?.url),
        stockQuantity: product.stockQuantity,
      },
      quantity,
    );
    setAdded(true);
    router.refresh();
  }

  if (!inStock) {
    return (
      <button
        type="button"
        disabled
        className="mt-8 w-full rounded-md bg-zinc-300 px-6 py-3 text-sm font-semibold text-white"
      >
        Indisponível
      </button>
    );
  }

  return (
    <div className="mt-8">
      <div className="flex items-center gap-3">
        <label htmlFor="quantity" className="text-sm text-zinc-600 dark:text-zinc-300">
          Quantidade
        </label>
        <input
          id="quantity"
          type="number"
          min={1}
          max={product.stockQuantity}
          value={quantity}
          onChange={(e) => setQuantity(Math.min(Math.max(Number(e.target.value) || 1, 1), product.stockQuantity))}
          className="w-20 rounded-md border border-zinc-300 px-2 py-1 text-sm dark:border-zinc-700 dark:bg-zinc-900"
        />
      </div>

      <button
        type="button"
        onClick={handleAdd}
        className="mt-3 w-full rounded-md bg-blue-600 px-6 py-3 text-sm font-semibold text-white hover:bg-blue-500"
      >
        {added ? "Adicionado ✓ — adicionar mais" : "Adicionar ao carrinho"}
      </button>
    </div>
  );
}
