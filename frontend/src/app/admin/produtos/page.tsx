import Link from "next/link";
import Form from "next/form";
import { getAuthToken } from "@/lib/auth";
import { serverApiFetch } from "@/lib/api";
import { formatKwanza } from "@/lib/format";
import type { PagedResult, ProductListItemDto } from "@/lib/types";
import { ProductStatusToggle } from "@/components/admin/ProductStatusToggle";

interface AdminProdutosPageProps {
  searchParams: Promise<{ search?: string; page?: string }>;
}

export default async function AdminProdutosPage({ searchParams }: AdminProdutosPageProps) {
  const resolved = await searchParams;
  const search = resolved.search ?? "";
  const page = Math.max(1, Number(resolved.page ?? "1") || 1);

  const token = await getAuthToken();
  const params = new URLSearchParams({ page: String(page), pageSize: "20", onlyActive: "false" });
  if (search) params.set("search", search);

  const result = await serverApiFetch<PagedResult<ProductListItemDto>>(
    `/api/products?${params.toString()}`,
    { token },
  );

  return (
    <div>
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-bold text-zinc-900 dark:text-white">Produtos</h1>
        <Link
          href="/admin/produtos/novo"
          className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-500"
        >
          Novo produto
        </Link>
      </div>

      <Form action="/admin/produtos" className="mt-4 flex max-w-sm gap-2">
        <input
          type="text"
          name="search"
          defaultValue={search}
          placeholder="Pesquisar por nome, SKU ou marca..."
          className="w-full rounded-md border border-zinc-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none dark:border-zinc-700 dark:bg-zinc-950"
        />
        <button
          type="submit"
          className="rounded-md border border-zinc-300 px-4 py-2 text-sm font-medium text-zinc-700 hover:border-zinc-400 dark:border-zinc-700 dark:text-zinc-200"
        >
          Buscar
        </button>
      </Form>

      <div className="mt-4 overflow-x-auto rounded-lg border border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-zinc-200 text-left text-xs uppercase tracking-wide text-zinc-400 dark:border-zinc-800">
              <th className="px-4 py-3">Produto</th>
              <th className="px-4 py-3">SKU</th>
              <th className="px-4 py-3">Categoria</th>
              <th className="px-4 py-3">Preço</th>
              <th className="px-4 py-3">Stock</th>
              <th className="px-4 py-3">Estado</th>
              <th className="px-4 py-3" />
            </tr>
          </thead>
          <tbody>
            {result.items.map((product) => (
              <tr key={product.id} className="border-b border-zinc-100 last:border-0 dark:border-zinc-900">
                <td className="px-4 py-3">
                  <Link
                    href={`/admin/produtos/${product.id}`}
                    className="font-medium text-zinc-900 hover:text-blue-600 dark:text-zinc-100"
                  >
                    {product.name}
                  </Link>
                </td>
                <td className="px-4 py-3 text-zinc-500">{product.sku}</td>
                <td className="px-4 py-3 text-zinc-500">{product.categoryName}</td>
                <td className="px-4 py-3 text-zinc-500">{formatKwanza(product.price)}</td>
                <td className="px-4 py-3 text-zinc-500">{product.stockQuantity}</td>
                <td className="px-4 py-3">
                  <ProductStatusToggle productId={product.id} isActive={product.isActive} />
                </td>
                <td className="px-4 py-3 text-right">
                  <Link
                    href={`/admin/produtos/${product.id}`}
                    className="text-sm font-medium text-blue-600 hover:underline"
                  >
                    Editar
                  </Link>
                </td>
              </tr>
            ))}
            {result.items.length === 0 && (
              <tr>
                <td colSpan={7} className="px-4 py-8 text-center text-zinc-400">
                  Nenhum produto encontrado.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {result.totalPages > 1 && (
        <p className="mt-4 text-sm text-zinc-500">
          Página {result.page} de {result.totalPages} ({result.totalCount} produtos)
        </p>
      )}
    </div>
  );
}
