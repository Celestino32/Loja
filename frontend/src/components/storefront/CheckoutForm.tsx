"use client";

import { useState, type FormEvent } from "react";
import { useRouter } from "next/navigation";
import { publicApiFetch } from "@/lib/api";
import { useCart } from "@/lib/cart-context";
import { formatKwanza } from "@/lib/format";
import type {
  CheckoutResultDto,
  CouponValidationResultDto,
  CustomerAddressDto,
  FulfillmentType,
  PaymentMethod,
  ShippingZoneDto,
} from "@/lib/types";

const paymentMethods: { value: PaymentMethod; label: string; description: string }[] = [
  {
    value: "MulticaixaExpress",
    label: "Multicaixa Express",
    description: "Confirme o pagamento na app Multicaixa Express.",
  },
  {
    value: "ReferenciaDePagamento",
    label: "Referência de pagamento",
    description: "Pague numa ATM ou Internet Banking com a referência gerada.",
  },
  {
    value: "TransferenciaBancaria",
    label: "Transferência bancária",
    description: "Transfira para a conta da loja e aguarde confirmação.",
  },
];

export function CheckoutForm({ zones, addresses }: { zones: ShippingZoneDto[]; addresses: CustomerAddressDto[] }) {
  const { items, totalPrice, clear } = useCart();
  const router = useRouter();

  const defaultAddress = addresses.find((a) => a.isDefault) ?? addresses[0];

  const [fulfillmentType, setFulfillmentType] = useState<FulfillmentType>("Entrega");
  const [zoneId, setZoneId] = useState(zones[0]?.id ?? "");
  const [street, setStreet] = useState(defaultAddress?.street ?? "");
  const [reference, setReference] = useState(defaultAddress?.reference ?? "");
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>("ReferenciaDePagamento");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [couponInput, setCouponInput] = useState("");
  const [appliedCoupon, setAppliedCoupon] = useState<{ code: string; discount: number } | null>(null);
  const [couponError, setCouponError] = useState<string | null>(null);
  const [isValidatingCoupon, setIsValidatingCoupon] = useState(false);

  const selectedZone = zones.find((z) => z.id === zoneId);
  const shippingCost = fulfillmentType === "Entrega" ? selectedZone?.cost ?? 0 : 0;
  const discount = appliedCoupon?.discount ?? 0;
  const total = totalPrice + shippingCost - discount;

  async function handleApplyCoupon() {
    if (!couponInput.trim()) return;
    setCouponError(null);
    setIsValidatingCoupon(true);
    try {
      const result = await publicApiFetch<CouponValidationResultDto>(
        `/api/coupons/validate?code=${encodeURIComponent(couponInput.trim())}&subtotal=${totalPrice}`,
      );
      if (!result.isValid) {
        setCouponError(result.message ?? "Cupom inválido.");
        setAppliedCoupon(null);
        return;
      }
      setAppliedCoupon({ code: couponInput.trim().toUpperCase(), discount: result.discountAmount });
    } catch {
      setCouponError("Não foi possível validar o cupom.");
      setAppliedCoupon(null);
    } finally {
      setIsValidatingCoupon(false);
    }
  }

  function handleRemoveCoupon() {
    setAppliedCoupon(null);
    setCouponInput("");
    setCouponError(null);
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);

    if (fulfillmentType === "Entrega" && (!zoneId || !street.trim())) {
      setError("Selecione a zona de entrega e informe o endereço.");
      return;
    }

    setIsSubmitting(true);
    try {
      const response = await fetch("/api/orders/checkout", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          items: items.map((i) => ({ productId: i.productId, quantity: i.quantity })),
          fulfillmentType,
          shippingZoneId: fulfillmentType === "Entrega" ? zoneId : null,
          shippingStreet: fulfillmentType === "Entrega" ? street.trim() : null,
          shippingReference: fulfillmentType === "Entrega" ? reference.trim() || null : null,
          paymentMethod,
          couponCode: appliedCoupon?.code ?? null,
        }),
      });

      if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.message ?? "Não foi possível finalizar a compra.");
      }

      const result = (await response.json()) as CheckoutResultDto;
      clear();
      router.push(`/minha-conta/pedidos/${result.orderId}`);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro inesperado.");
    } finally {
      setIsSubmitting(false);
    }
  }

  if (items.length === 0) {
    return (
      <p className="mt-6 text-sm text-zinc-500">
        O seu carrinho está vazio. Volte à loja para adicionar produtos antes de finalizar a compra.
      </p>
    );
  }

  return (
    <form onSubmit={handleSubmit} className="mt-6 flex flex-col gap-8">
      <section className="rounded-lg border border-zinc-200 p-4 dark:border-zinc-800">
        <h2 className="font-semibold text-zinc-900 dark:text-white">Itens</h2>
        <ul className="mt-3 space-y-2 text-sm">
          {items.map((item) => (
            <li key={item.productId} className="flex justify-between text-zinc-600 dark:text-zinc-300">
              <span>
                {item.quantity}× {item.name}
              </span>
              <span>{formatKwanza(item.price * item.quantity)}</span>
            </li>
          ))}
        </ul>
      </section>

      <section className="rounded-lg border border-zinc-200 p-4 dark:border-zinc-800">
        <h2 className="font-semibold text-zinc-900 dark:text-white">Entrega</h2>

        <div className="mt-3 flex gap-4 text-sm">
          <label className="flex items-center gap-2">
            <input
              type="radio"
              checked={fulfillmentType === "Entrega"}
              onChange={() => setFulfillmentType("Entrega")}
            />
            Entrega
          </label>
          <label className="flex items-center gap-2">
            <input
              type="radio"
              checked={fulfillmentType === "RetiradaNaLoja"}
              onChange={() => setFulfillmentType("RetiradaNaLoja")}
            />
            Retirar na loja
          </label>
        </div>

        {fulfillmentType === "Entrega" && (
          <div className="mt-4 flex flex-col gap-3">
            <div>
              <label className="text-sm font-medium text-zinc-700 dark:text-zinc-200">
                Província / Município
              </label>
              <select
                value={zoneId}
                onChange={(e) => setZoneId(e.target.value)}
                className="mt-1 w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-900"
              >
                {zones.map((zone) => (
                  <option key={zone.id} value={zone.id}>
                    {zone.province} — {zone.municipality} ({formatKwanza(zone.cost)})
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="text-sm font-medium text-zinc-700 dark:text-zinc-200">Endereço</label>
              <input
                value={street}
                onChange={(e) => setStreet(e.target.value)}
                placeholder="Rua, bairro, nº da casa"
                className="mt-1 w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-900"
              />
            </div>

            <div>
              <label className="text-sm font-medium text-zinc-700 dark:text-zinc-200">
                Ponto de referência (opcional)
              </label>
              <input
                value={reference}
                onChange={(e) => setReference(e.target.value)}
                className="mt-1 w-full rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-900"
              />
            </div>
          </div>
        )}
      </section>

      <section className="rounded-lg border border-zinc-200 p-4 dark:border-zinc-800">
        <h2 className="font-semibold text-zinc-900 dark:text-white">Pagamento</h2>
        <div className="mt-3 flex flex-col gap-2">
          {paymentMethods.map((method) => (
            <label
              key={method.value}
              className="flex cursor-pointer items-start gap-3 rounded-md border border-zinc-200 p-3 text-sm hover:border-blue-400 dark:border-zinc-800"
            >
              <input
                type="radio"
                className="mt-1"
                checked={paymentMethod === method.value}
                onChange={() => setPaymentMethod(method.value)}
              />
              <span>
                <span className="block font-medium text-zinc-900 dark:text-zinc-100">{method.label}</span>
                <span className="block text-zinc-500">{method.description}</span>
              </span>
            </label>
          ))}
        </div>
      </section>

      <section className="rounded-lg border border-zinc-200 p-4 dark:border-zinc-800">
        <h2 className="font-semibold text-zinc-900 dark:text-white">Cupom de desconto</h2>
        {appliedCoupon ? (
          <div className="mt-3 flex items-center justify-between rounded-md bg-emerald-50 px-3 py-2 text-sm dark:bg-emerald-950">
            <span className="text-emerald-700 dark:text-emerald-300">
              Cupom <strong>{appliedCoupon.code}</strong> aplicado: -{formatKwanza(appliedCoupon.discount)}
            </span>
            <button type="button" onClick={handleRemoveCoupon} className="text-emerald-700 hover:underline dark:text-emerald-300">
              Remover
            </button>
          </div>
        ) : (
          <div className="mt-3 flex gap-2">
            <input
              value={couponInput}
              onChange={(e) => setCouponInput(e.target.value)}
              placeholder="Código do cupom"
              className="w-full max-w-xs rounded-md border border-zinc-300 px-3 py-2 text-sm uppercase dark:border-zinc-700 dark:bg-zinc-900"
            />
            <button
              type="button"
              onClick={handleApplyCoupon}
              disabled={isValidatingCoupon}
              className="rounded-md border border-zinc-300 px-4 py-2 text-sm font-medium text-zinc-700 hover:border-zinc-400 disabled:opacity-50 dark:border-zinc-700 dark:text-zinc-200"
            >
              {isValidatingCoupon ? "A validar..." : "Aplicar"}
            </button>
          </div>
        )}
        {couponError && <p className="mt-2 text-sm text-red-600">{couponError}</p>}
      </section>

      <section className="flex items-center justify-between border-t border-zinc-200 pt-4 dark:border-zinc-800">
        <div className="text-sm text-zinc-500">
          <p>Subtotal: {formatKwanza(totalPrice)}</p>
          <p>Frete: {formatKwanza(shippingCost)}</p>
          {discount > 0 && <p className="text-emerald-600">Desconto: -{formatKwanza(discount)}</p>}
        </div>
        <p className="text-xl font-bold text-zinc-900 dark:text-white">{formatKwanza(total)}</p>
      </section>

      {error && <p className="text-sm text-red-600">{error}</p>}

      <button
        type="submit"
        disabled={isSubmitting}
        className="rounded-md bg-blue-600 px-6 py-3 text-sm font-semibold text-white hover:bg-blue-500 disabled:bg-zinc-300"
      >
        {isSubmitting ? "A processar..." : "Confirmar pedido"}
      </button>
    </form>
  );
}
