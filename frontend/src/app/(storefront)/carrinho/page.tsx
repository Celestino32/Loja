"use client";

import Link from "next/link";
import { useCart } from "@/lib/cart-context";
import { formatKwanza } from "@/lib/format";

export default function CarrinhoPage() {
  const { items, removeItem, setQuantity, totalPrice } = useCart();

  if (items.length === 0) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-16 text-center sm:px-6">
        <h1 className="text-2xl font-bold text-zinc-900 dark:text-white">O seu carrinho está vazio</h1>
        <Link href="/produtos" className="mt-4 inline-block text-blue-600 hover:underline">
          Ver produtos
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-3xl px-4 py-10 sm:px-6">
      <h1 className="text-2xl font-bold text-zinc-900 dark:text-white">Carrinho</h1>

      <div className="mt-6 divide-y divide-zinc-200 rounded-lg border border-zinc-200 dark:divide-zinc-800 dark:border-zinc-800">
        {items.map((item) => (
          <div key={item.productId} className="flex items-center gap-4 p-4">
            <div className="flex h-16 w-16 shrink-0 items-center justify-center rounded-md bg-zinc-100 dark:bg-zinc-800">
              {item.imageUrl ? (
                // eslint-disable-next-line @next/next/no-img-element
                <img src={item.imageUrl} alt={item.name} className="h-full w-full rounded-md object-cover" />
              ) : (
                <span className="text-xs text-zinc-400">Sem foto</span>
              )}
            </div>

            <div className="flex-1">
              <Link href={`/produtos/${item.slug}`} className="font-medium text-zinc-900 hover:text-blue-600 dark:text-zinc-100">
                {item.name}
              </Link>
              <p className="text-sm text-zinc-500">{formatKwanza(item.price)} / unidade</p>
            </div>

            <input
              type="number"
              min={1}
              max={item.stockQuantity}
              value={item.quantity}
              onChange={(e) => setQuantity(item.productId, Number(e.target.value) || 1)}
              className="w-16 rounded-md border border-zinc-300 px-2 py-1 text-sm dark:border-zinc-700 dark:bg-zinc-900"
            />

            <p className="w-28 text-right font-medium text-zinc-900 dark:text-zinc-100">
              {formatKwanza(item.price * item.quantity)}
            </p>

            <button
              type="button"
              onClick={() => removeItem(item.productId)}
              className="text-sm text-red-600 hover:underline"
            >
              Remover
            </button>
          </div>
        ))}
      </div>

      <div className="mt-6 flex items-center justify-between">
        <span className="text-lg font-semibold text-zinc-900 dark:text-white">Subtotal</span>
        <span className="text-lg font-bold text-zinc-900 dark:text-white">{formatKwanza(totalPrice)}</span>
      </div>
      <p className="mt-1 text-right text-xs text-zinc-400">O frete é calculado no checkout.</p>

      <Link
        href="/checkout"
        className="mt-6 block w-full rounded-md bg-blue-600 px-6 py-3 text-center text-sm font-semibold text-white hover:bg-blue-500"
      >
        Finalizar compra
      </Link>
    </div>
  );
}
