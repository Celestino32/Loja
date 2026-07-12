import { formatKwanza } from "@/lib/format";
import type { DailySalesDto } from "@/lib/types";

export function SalesBarChart({ data }: { data: DailySalesDto[] }) {
  if (data.length === 0) {
    return <p className="text-sm text-zinc-500">Sem vendas no período selecionado.</p>;
  }

  const max = Math.max(...data.map((d) => d.revenue), 1);

  return (
    <div className="flex h-48 items-end gap-1 overflow-x-auto pb-1">
      {data.map((day) => (
        <div key={day.date} className="group relative flex h-full flex-1 min-w-[8px] flex-col justify-end">
          <div
            className="w-full rounded-t bg-blue-500 transition-colors group-hover:bg-blue-600"
            style={{ height: `${Math.max((day.revenue / max) * 100, 2)}%` }}
          />
          <div className="pointer-events-none absolute bottom-full left-1/2 z-10 mb-1 w-max -translate-x-1/2 rounded bg-zinc-900 px-2 py-1 text-xs text-white opacity-0 transition-opacity group-hover:opacity-100">
            {day.date}: {formatKwanza(day.revenue)} ({day.orderCount} pedido{day.orderCount === 1 ? "" : "s"})
          </div>
        </div>
      ))}
    </div>
  );
}
