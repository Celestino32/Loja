"use client";

import Link from "next/link";
import { useCart } from "@/lib/cart-context";

export function CartBadge() {
  const { totalItems } = useCart();

  return (
    <Link
      href="/carrinho"
      className="relative rounded-md border border-zinc-300 px-3 py-2 text-sm font-medium text-zinc-700 hover:border-zinc-400 dark:border-zinc-700 dark:text-zinc-200"
    >
      Carrinho
      {totalItems > 0 && (
        <span className="absolute -right-2 -top-2 flex h-5 w-5 items-center justify-center rounded-full bg-blue-600 text-xs font-bold text-white">
          {totalItems}
        </span>
      )}
    </Link>
  );
}
