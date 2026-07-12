import { getAuthToken } from "@/lib/auth";
import { serverApiFetch } from "@/lib/api";
import { formatKwanza } from "@/lib/format";
import type { ShippingZoneDto } from "@/lib/types";
import { NewShippingZoneForm } from "@/components/admin/NewShippingZoneForm";
import { ShippingZoneStatusToggle } from "@/components/admin/ShippingZoneStatusToggle";

export default async function AdminFretePage() {
  const token = await getAuthToken();
  const zones = await serverApiFetch<ShippingZoneDto[]>("/api/shipping-zones?onlyActive=false", { token });

  return (
    <div>
      <h1 className="text-xl font-bold text-zinc-900 dark:text-white">Zonas de frete</h1>
      <p className="mt-1 text-sm text-zinc-500">
        Custo de entrega por província/município, usado no checkout da loja.
      </p>

      <div className="mt-4 rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <NewShippingZoneForm />
      </div>

      <div className="mt-6 overflow-x-auto rounded-lg border border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-zinc-200 text-left text-xs uppercase tracking-wide text-zinc-400 dark:border-zinc-800">
              <th className="px-4 py-3">Província</th>
              <th className="px-4 py-3">Município</th>
              <th className="px-4 py-3">Custo</th>
              <th className="px-4 py-3">Estado</th>
            </tr>
          </thead>
          <tbody>
            {zones.map((zone) => (
              <tr key={zone.id} className="border-b border-zinc-100 last:border-0 dark:border-zinc-900">
                <td className="px-4 py-3 font-medium text-zinc-900 dark:text-zinc-100">{zone.province}</td>
                <td className="px-4 py-3 text-zinc-500">{zone.municipality}</td>
                <td className="px-4 py-3 text-zinc-500">{formatKwanza(zone.cost)}</td>
                <td className="px-4 py-3">
                  <ShippingZoneStatusToggle zoneId={zone.id} isActive={zone.isActive} />
                </td>
              </tr>
            ))}
            {zones.length === 0 && (
              <tr>
                <td colSpan={4} className="px-4 py-8 text-center text-zinc-400">
                  Nenhuma zona de frete cadastrada.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
