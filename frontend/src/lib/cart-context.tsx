"use client";

import { createContext, useContext, useEffect, useState, type ReactNode } from "react";

export interface CartItem {
  productId: string;
  name: string;
  slug: string;
  price: number;
  quantity: number;
  imageUrl: string | null;
  stockQuantity: number;
}

interface CartContextValue {
  items: CartItem[];
  addItem: (item: Omit<CartItem, "quantity">, quantity?: number) => void;
  removeItem: (productId: string) => void;
  setQuantity: (productId: string, quantity: number) => void;
  clear: () => void;
  totalItems: number;
  totalPrice: number;
}

const CartContext = createContext<CartContextValue | null>(null);
const STORAGE_KEY = "lv_cart";

export function CartProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<CartItem[]>([]);
  const [hydrated, setHydrated] = useState(false);

  useEffect(() => {
    try {
      const raw = window.localStorage.getItem(STORAGE_KEY);
      if (raw) setItems(JSON.parse(raw));
    } catch {
      // localStorage indisponível ou dados corrompidos: começa com carrinho vazio.
    } finally {
      setHydrated(true);
    }
  }, []);

  useEffect(() => {
    if (!hydrated) return;
    window.localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
  }, [items, hydrated]);

  function addItem(item: Omit<CartItem, "quantity">, quantity = 1) {
    setItems((current) => {
      const existing = current.find((i) => i.productId === item.productId);
      const maxQuantity = item.stockQuantity;

      if (existing) {
        const nextQuantity = Math.min(existing.quantity + quantity, maxQuantity);
        return current.map((i) => (i.productId === item.productId ? { ...i, quantity: nextQuantity } : i));
      }

      return [...current, { ...item, quantity: Math.min(quantity, maxQuantity) }];
    });
  }

  function removeItem(productId: string) {
    setItems((current) => current.filter((i) => i.productId !== productId));
  }

  function setQuantity(productId: string, quantity: number) {
    setItems((current) =>
      current
        .map((i) => (i.productId === productId ? { ...i, quantity: Math.min(Math.max(quantity, 1), i.stockQuantity) } : i))
        .filter((i) => i.quantity > 0),
    );
  }

  function clear() {
    setItems([]);
  }

  const totalItems = items.reduce((sum, i) => sum + i.quantity, 0);
  const totalPrice = items.reduce((sum, i) => sum + i.quantity * i.price, 0);

  return (
    <CartContext.Provider value={{ items, addItem, removeItem, setQuantity, clear, totalItems, totalPrice }}>
      {children}
    </CartContext.Provider>
  );
}

export function useCart(): CartContextValue {
  const context = useContext(CartContext);
  if (!context) throw new Error("useCart deve ser usado dentro de um CartProvider.");
  return context;
}
