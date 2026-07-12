"use client";

import { useTransition } from "react";
import { setShippingZoneActive } from "@/app/admin/actions";

export function ShippingZoneStatusToggle({ zoneId, isActive }: { zoneId: string; isActive: boolean }) {
  const [isPending, startTransition] = useTransition();

  function toggle() {
    startTransition(() => setShippingZoneActive(zoneId, !isActive));
  }

  return (
    <button
      type="button"
      onClick={toggle}
      disabled={isPending}
      className={`rounded-full px-2 py-0.5 text-xs font-medium ${
        isActive ? "bg-emerald-100 text-emerald-700" : "bg-zinc-200 text-zinc-600"
      }`}
    >
      {isActive ? "Ativa" : "Inativa"}
    </button>
  );
}
