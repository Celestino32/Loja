import Link from "next/link";
import Form from "next/form";
import { getAuthToken } from "@/lib/auth";
import { serverApiFetch } from "@/lib/api";
import { formatKwanza } from "@/lib/format";
import type { LowStockProductDto, SalesReportDto, TopProductDto } from "@/lib/types";
import { SalesBarChart } from "@/components/admin/SalesBarChart";

interface AdminRelatoriosPageProps {
  searchParams: Promise<{ from?: string; to?: string }>;
}

function toDateInput(date: Date) {
  return date.toISOString().slice(0, 10);
}

export default async function AdminRelatoriosPage({ searchParams }: AdminRelatoriosPageProps) {
  const resolved = await searchParams;

  const today = new Date();
  const defaultFrom = new Date(today);
  defaultFrom.setDate(defaultFrom.getDate() - 29);

  const from = resolved.from || toDateInput(defaultFrom);
  const to = resolved.to || toDateInput(today);

  const token = await getAuthToken();

  const [sales, topProducts, lowStock] = await Promise.all([
    serverApiFetch<SalesReportDto>(`/api/reports/sales?from=${from}&to=${to}`, { token }),
    serverApiFetch<TopProductDto[]>(`/api/reports/top-products?from=${from}&to=${to}&limit=10`, { token }),
    serverApiFetch<LowStockProductDto[]>("/api/reports/low-stock?threshold=5", { token }),
  ]);

  return (
    <div>
      <h1 className="text-xl font-bold text-zinc-900 dark:text-white">Relatórios</h1>

      <Form action="/admin/relatorios" className="mt-4 flex flex-wrap items-end gap-3">
        <div>
          <label className="text-xs font-medium text-zinc-500">De</label>
          <input
            type="date"
            name="from"
            defaultValue={from}
            className="mt-1 block rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
          />
        </div>
        <div>
          <label className="text-xs font-medium text-zinc-500">Até</label>
          <input
            type="date"
            name="to"
            defaultValue={to}
            className="mt-1 block rounded-md border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
          />
        </div>
        <button
          type="submit"
          className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-500"
        >
          Filtrar
        </button>
        <a
          href={`/api/invoices/export/saft?from=${from}&to=${to}`}
          className="rounded-md border border-zinc-300 px-4 py-2 text-sm font-medium text-zinc-700 hover:bg-zinc-50 dark:border-zinc-700 dark:text-zinc-200 dark:hover:bg-zinc-900"
        >
          Exportar SAF-T (AO)
        </a>
      </Form>
      <p className="mt-2 text-xs text-zinc-400">
        Exportação best-effort no formato SAF-T; ainda não validada contra o XSD oficial da AGT nem certificada (ver detalhes com o time técnico antes de submeter à Administração Geral Tributária).
      </p>

      <div className="mt-6 grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div className="rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
          <p className="text-xs font-medium uppercase tracking-wide text-zinc-400">Receita no período</p>
          <p className="mt-2 text-2xl font-bold text-zinc-900 dark:text-white">{formatKwanza(sales.totalRevenue)}</p>
        </div>
        <div className="rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
          <p className="text-xs font-medium uppercase tracking-wide text-zinc-400">Pedidos pagos</p>
          <p className="mt-2 text-2xl font-bold text-zinc-900 dark:text-white">{sales.orderCount}</p>
        </div>
        <div className="rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
          <p className="text-xs font-medium uppercase tracking-wide text-zinc-400">Ticket médio</p>
          <p className="mt-2 text-2xl font-bold text-zinc-900 dark:text-white">{formatKwanza(sales.averageOrderValue)}</p>
        </div>
      </div>

      <div className="mt-6 rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <h2 className="mb-4 font-semibold text-zinc-900 dark:text-white">Vendas por dia</h2>
        <SalesBarChart data={sales.dailyBreakdown} />
      </div>

      <div className="mt-6 grid gap-6 lg:grid-cols-2">
        <div className="rounded-lg border border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
          <div className="border-b border-zinc-200 px-4 py-3 dark:border-zinc-800">
            <h2 className="font-semibold text-zinc-900 dark:text-white">Produtos mais vendidos</h2>
          </div>
          <table className="w-full text-sm">
            <tbody>
              {topProducts.map((product) => (
                <tr key={product.productId} className="border-b border-zinc-100 last:border-0 dark:border-zinc-900">
                  <td className="px-4 py-3 text-zinc-900 dark:text-zinc-100">{product.productName}</td>
                  <td className="px-4 py-3 text-right text-zinc-500">{product.quantitySold} un.</td>
                  <td className="px-4 py-3 text-right font-medium text-zinc-900 dark:text-zinc-100">
                    {formatKwanza(product.revenue)}
                  </td>
                </tr>
              ))}
              {topProducts.length === 0 && (
                <tr>
                  <td colSpan={3} className="px-4 py-8 text-center text-zinc-400">
                    Nenhuma venda no período.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        <div className="rounded-lg border border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
          <div className="border-b border-zinc-200 px-4 py-3 dark:border-zinc-800">
            <h2 className="font-semibold text-zinc-900 dark:text-white">Estoque baixo (≤ 5 unidades)</h2>
          </div>
          <table className="w-full text-sm">
            <tbody>
              {lowStock.map((product) => (
                <tr key={product.productId} className="border-b border-zinc-100 last:border-0 dark:border-zinc-900">
                  <td className="px-4 py-3">
                    <Link
                      href={`/admin/produtos/${product.productId}`}
                      className="font-medium text-zinc-900 hover:text-blue-600 dark:text-zinc-100"
                    >
                      {product.productName}
                    </Link>
                    <p className="text-xs text-zinc-400">{product.sku}</p>
                  </td>
                  <td className="px-4 py-3 text-right font-medium text-red-600">{product.stockQuantity} un.</td>
                </tr>
              ))}
              {lowStock.length === 0 && (
                <tr>
                  <td colSpan={2} className="px-4 py-8 text-center text-zinc-400">
                    Nenhum produto com estoque baixo.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
