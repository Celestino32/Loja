import { redirect } from "next/navigation";
import { getAuthToken, getServerSession } from "@/lib/auth";
import { serverApiFetch } from "@/lib/api";
import type { CustomerDto, ShippingZoneDto } from "@/lib/types";
import { CheckoutForm } from "@/components/storefront/CheckoutForm";

export default async function CheckoutPage() {
  const session = await getServerSession();
  if (!session) redirect("/login?redirectTo=/checkout");

  const token = await getAuthToken();

  const [zones, profile] = await Promise.all([
    serverApiFetch<ShippingZoneDto[]>("/api/shipping-zones?onlyActive=true"),
    serverApiFetch<CustomerDto>("/api/customers/me", { token }),
  ]);

  return (
    <div className="mx-auto max-w-3xl px-4 py-10 sm:px-6">
      <h1 className="text-2xl font-bold text-zinc-900 dark:text-white">Finalizar compra</h1>
      <CheckoutForm zones={zones} addresses={profile.addresses} />
    </div>
  );
}
