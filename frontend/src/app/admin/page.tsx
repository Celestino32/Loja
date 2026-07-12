import Link from "next/link";
import { getAuthToken } from "@/lib/auth";
import { serverApiFetch } from "@/lib/api";
import { formatKwanza } from "@/lib/format";
import type { CategoryDto, PagedResult, ProductListItemDto } from "@/lib/types";

async function getStats() {
  const token = await getAuthToken();

  const [allProducts, activeProducts, categories] = await Promise.all([
    serverApiFetch<PagedResult<ProductListItemDto>>("/api/products?onlyActive=false&pageSize=100", { token }),
    serverApiFetch<PagedResult<ProductListItemDto>>("/api/products?onlyActive=true&pageSize=1", { token }),
    serverApiFetch<CategoryDto[]>("/api/categories?onlyActive=false", { token }),
  ]);

  return {
    totalProducts: allProducts.totalCount,
    activeProducts: activeProducts.totalCount,
    outOfStock: allProducts.items.filter((p) => p.stockQuantity === 0).length,
    totalCategories: categories.length,
    recentProducts: allProducts.items.slice(0, 5),
  };
}

export default async function AdminDashboardPage() {
  const stats = await getStats();

  const cards = [
    { label: "Produtos ativos", value: stats.activeProducts },
    { label: "Total de produtos", value: stats.totalProducts },
    { label: "Sem stock", value: stats.outOfStock },
    { label: "Categorias", value: stats.totalCategories },
  ];

  return (
    <div>
      <h1 className="text-xl font-bold text-zinc-900 dark:text-white">Dashboard</h1>

      <div className="mt-6 grid grid-cols-2 gap-4 lg:grid-cols-4">
        {cards.map((card) => (
          <div
            key={card.label}
            className="rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950"
          >
            <p className="text-xs font-medium uppercase tracking-wide text-zinc-400">
              {card.label}
            </p>
            <p className="mt-2 text-2xl font-bold text-zinc-900 dark:text-white">{card.value}</p>
          </div>
        ))}
      </div>

      <div className="mt-8 rounded-lg border border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
        <div className="flex items-center justify-between border-b border-zinc-200 px-4 py-3 dark:border-zinc-800">
          <h2 className="text-sm font-semibold text-zinc-900 dark:text-white">
            Produtos recentes
          </h2>
          <Link href="/admin/produtos" className="text-sm font-medium text-blue-600 hover:underline">
            Ver todos
          </Link>
        </div>
        <table className="w-full text-sm">
          <tbody>
            {stats.recentProducts.map((product) => (
              <tr key={product.id} className="border-b border-zinc-100 last:border-0 dark:border-zinc-900">
                <td className="px-4 py-3 font-medium text-zinc-900 dark:text-zinc-100">
                  {product.name}
                </td>
                <td className="px-4 py-3 text-zinc-500">{product.categoryName}</td>
                <td className="px-4 py-3 text-zinc-500">{formatKwanza(product.price)}</td>
                <td className="px-4 py-3">
                  <span
                    className={`rounded-full px-2 py-0.5 text-xs font-medium ${
                      product.isActive
                        ? "bg-emerald-100 text-emerald-700"
                        : "bg-zinc-100 text-zinc-500"
                    }`}
                  >
                    {product.isActive ? "Ativo" : "Inativo"}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
