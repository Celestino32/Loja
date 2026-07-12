import Link from "next/link";
import Form from "next/form";
import { serverApiFetch } from "@/lib/api";
import { ProductCard } from "@/components/storefront/ProductCard";
import type { CategoryDto, PagedResult, ProductListItemDto } from "@/lib/types";

interface ProdutosPageProps {
  searchParams: Promise<{ search?: string; categoryId?: string; page?: string }>;
}

async function getProducts(search: string, categoryId: string, page: number) {
  const params = new URLSearchParams({ page: String(page), pageSize: "12" });
  if (search) params.set("search", search);
  if (categoryId) params.set("categoryId", categoryId);

  return serverApiFetch<PagedResult<ProductListItemDto>>(`/api/products?${params.toString()}`);
}

async function getCategories() {
  return serverApiFetch<CategoryDto[]>("/api/categories");
}

function buildPageHref(search: string, categoryId: string, page: number) {
  const params = new URLSearchParams({ page: String(page) });
  if (search) params.set("search", search);
  if (categoryId) params.set("categoryId", categoryId);
  return `/produtos?${params.toString()}`;
}

export default async function ProdutosPage({ searchParams }: ProdutosPageProps) {
  const resolved = await searchParams;
  const search = resolved.search ?? "";
  const categoryId = resolved.categoryId ?? "";
  const page = Math.max(1, Number(resolved.page ?? "1") || 1);

  const [result, categories] = await Promise.all([
    getProducts(search, categoryId, page),
    getCategories(),
  ]);

  return (
    <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6">
      <h1 className="text-2xl font-bold text-zinc-900 dark:text-white">Produtos</h1>

      <div className="mt-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <Form action="/produtos" className="flex w-full max-w-md gap-2">
          {categoryId && <input type="hidden" name="categoryId" value={categoryId} />}
          <input
            type="text"
            name="search"
            defaultValue={search}
            placeholder="Pesquisar produtos..."
            className="w-full rounded-md border border-zinc-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none dark:border-zinc-700 dark:bg-zinc-900"
          />
          <button
            type="submit"
            className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-500"
          >
            Buscar
          </button>
        </Form>

        <div className="flex flex-wrap gap-2">
          <Link
            href="/produtos"
            className={`rounded-full px-3 py-1 text-xs font-medium ${
              !categoryId
                ? "bg-blue-600 text-white"
                : "bg-zinc-100 text-zinc-600 dark:bg-zinc-800 dark:text-zinc-300"
            }`}
          >
            Todas
          </Link>
          {categories.map((category) => (
            <Link
              key={category.id}
              href={`/produtos?categoryId=${category.id}`}
              className={`rounded-full px-3 py-1 text-xs font-medium ${
                categoryId === category.id
                  ? "bg-blue-600 text-white"
                  : "bg-zinc-100 text-zinc-600 dark:bg-zinc-800 dark:text-zinc-300"
              }`}
            >
              {category.name}
            </Link>
          ))}
        </div>
      </div>

      {result.items.length === 0 ? (
        <p className="mt-10 text-sm text-zinc-500">
          Nenhum produto encontrado para os filtros selecionados.
        </p>
      ) : (
        <>
          <div className="mt-8 grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
            {result.items.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>

          {result.totalPages > 1 && (
            <div className="mt-10 flex items-center justify-center gap-4 text-sm">
              <Link
                aria-disabled={page <= 1}
                className={`rounded-md border px-3 py-2 ${
                  page <= 1
                    ? "pointer-events-none border-zinc-200 text-zinc-300"
                    : "border-zinc-300 text-zinc-700 hover:border-zinc-400"
                }`}
                href={buildPageHref(search, categoryId, page - 1)}
              >
                Anterior
              </Link>
              <span className="text-zinc-500">
                Página {result.page} de {result.totalPages}
              </span>
              <Link
                aria-disabled={page >= result.totalPages}
                className={`rounded-md border px-3 py-2 ${
                  page >= result.totalPages
                    ? "pointer-events-none border-zinc-200 text-zinc-300"
                    : "border-zinc-300 text-zinc-700 hover:border-zinc-400"
                }`}
                href={buildPageHref(search, categoryId, page + 1)}
              >
                Próxima
              </Link>
            </div>
          )}
        </>
      )}
    </div>
  );
}
