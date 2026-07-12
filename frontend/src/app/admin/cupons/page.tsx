import { getAuthToken } from "@/lib/auth";
import { serverApiFetch } from "@/lib/api";
import { formatDateTime, formatKwanza } from "@/lib/format";
import type { CouponDto } from "@/lib/types";
import { NewCouponForm } from "@/components/admin/NewCouponForm";
import { CouponStatusToggle } from "@/components/admin/CouponStatusToggle";

function formatDiscount(coupon: CouponDto) {
  return coupon.discountType === "Percentual" ? `${coupon.discountValue}%` : formatKwanza(coupon.discountValue);
}

export default async function AdminCuponsPage() {
  const token = await getAuthToken();
  const coupons = await serverApiFetch<CouponDto[]>("/api/coupons", { token });

  return (
    <div>
      <h1 className="text-xl font-bold text-zinc-900 dark:text-white">Cupons de desconto</h1>
      <p className="mt-1 text-sm text-zinc-500">
        Cupons são aplicados sobre o subtotal dos itens no checkout, antes do frete.
      </p>

      <div className="mt-4 rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <NewCouponForm />
      </div>

      <div className="mt-6 overflow-x-auto rounded-lg border border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-zinc-200 text-left text-xs uppercase tracking-wide text-zinc-400 dark:border-zinc-800">
              <th className="px-4 py-3">Código</th>
              <th className="px-4 py-3">Desconto</th>
              <th className="px-4 py-3">Pedido mínimo</th>
              <th className="px-4 py-3">Usos</th>
              <th className="px-4 py-3">Válido até</th>
              <th className="px-4 py-3">Estado</th>
            </tr>
          </thead>
          <tbody>
            {coupons.map((coupon) => (
              <tr key={coupon.id} className="border-b border-zinc-100 last:border-0 dark:border-zinc-900">
                <td className="px-4 py-3 font-medium text-zinc-900 dark:text-zinc-100">{coupon.code}</td>
                <td className="px-4 py-3 text-zinc-500">{formatDiscount(coupon)}</td>
                <td className="px-4 py-3 text-zinc-500">
                  {coupon.minOrderValue ? formatKwanza(coupon.minOrderValue) : "—"}
                </td>
                <td className="px-4 py-3 text-zinc-500">
                  {coupon.usedCount}
                  {coupon.maxUses ? ` / ${coupon.maxUses}` : ""}
                </td>
                <td className="px-4 py-3 text-zinc-500">
                  {coupon.validUntil ? formatDateTime(coupon.validUntil) : "Sem expiração"}
                </td>
                <td className="px-4 py-3">
                  <CouponStatusToggle couponId={coupon.id} isActive={coupon.isActive} />
                </td>
              </tr>
            ))}
            {coupons.length === 0 && (
              <tr>
                <td colSpan={6} className="px-4 py-8 text-center text-zinc-400">
                  Nenhum cupom cadastrado.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
