import Link from "next/link";
import { serverApiFetch } from "@/lib/api";
import { HeroCarousel } from "@/components/storefront/HeroCarousel";
import { ProductCard } from "@/components/storefront/ProductCard";
import { TrustBadges } from "@/components/storefront/TrustBadges";
import type { CategoryDto, PagedResult, ProductListItemDto } from "@/lib/types";

async function getFeaturedProducts() {
  return serverApiFetch<PagedResult<ProductListItemDto>>("/api/products?page=1&pageSize=8");
}

async function getCategories() {
  return serverApiFetch<CategoryDto[]>("/api/categories");
}

export default async function HomePage() {
  const [products, categories] = await Promise.all([getFeaturedProducts(), getCategories()]);

  return (
    <div>
      <HeroCarousel />
      <TrustBadges />

      {categories.length > 0 && (
        <section className="mx-auto max-w-6xl px-4 py-10 sm:px-6">
          <h2 className="text-lg font-semibold text-zinc-900 dark:text-white">Categorias</h2>
          <div className="mt-4 grid grid-cols-2 gap-4 sm:grid-cols-3">
            {categories.map((category) => (
              <Link
                key={category.id}
                href={`/produtos?categoryId=${category.id}`}
                className="rounded-lg border border-zinc-200 p-4 text-center text-sm font-medium text-zinc-700 hover:border-blue-500 hover:text-blue-600 dark:border-zinc-800 dark:text-zinc-200"
              >
                {category.name}
              </Link>
            ))}
          </div>
        </section>
      )}

      <section className="mx-auto max-w-6xl px-4 pb-16 sm:px-6">
        <div className="flex items-center justify-between">
          <h2 className="text-lg font-semibold text-zinc-900 dark:text-white">
            Produtos em destaque
          </h2>
          <Link href="/produtos" className="text-sm font-medium text-blue-600 hover:underline">
            Ver todos
          </Link>
        </div>

        {products.items.length === 0 ? (
          <p className="mt-6 text-sm text-zinc-500">Nenhum produto disponível no momento.</p>
        ) : (
          <div className="mt-6 grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
            {products.items.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
