import { notFound } from "next/navigation";
import { ApiError, serverApiFetch } from "@/lib/api";
import { formatKwanza } from "@/lib/format";
import type { ProductDto } from "@/lib/types";
import { AddToCartButton } from "@/components/storefront/AddToCartButton";
import { ProductImageCarousel } from "@/components/storefront/ProductImageCarousel";
import { ProductReviews } from "@/components/storefront/ProductReviews";

interface ProdutoPageProps {
  params: Promise<{ slug: string }>;
}

async function getProduct(slug: string): Promise<ProductDto | null> {
  try {
    return await serverApiFetch<ProductDto>(`/api/products/slug/${slug}`);
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) return null;
    throw error;
  }
}

export default async function ProdutoPage({ params }: ProdutoPageProps) {
  const { slug } = await params;
  const product = await getProduct(slug);

  if (!product) notFound();

  const inStock = product.stockQuantity > 0;

  return (
    <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6">
      <div className="grid gap-10 md:grid-cols-2">
        <ProductImageCarousel images={product.images} fallbackLabel={product.brand} alt={product.name} />

        <div>
          {product.categoryName && (
            <span className="text-xs font-medium uppercase tracking-wide text-blue-600">
              {product.categoryName}
            </span>
          )}
          <h1 className="mt-1 text-2xl font-bold text-zinc-900 dark:text-white">
            {product.name}
          </h1>
          <p className="mt-1 text-sm text-zinc-500">
            Marca: {product.brand} · SKU: {product.sku}
          </p>

          <p className="mt-6 text-3xl font-bold text-zinc-900 dark:text-white">
            {formatKwanza(product.price)}
          </p>

          <p className={`mt-2 text-sm font-medium ${inStock ? "text-emerald-600" : "text-red-600"}`}>
            {inStock ? `${product.stockQuantity} unidades em stock` : "Produto esgotado"}
          </p>

          {product.description && (
            <p className="mt-6 text-sm leading-relaxed text-zinc-600 dark:text-zinc-300">
              {product.description}
            </p>
          )}

          {product.attributes.length > 0 && (
            <dl className="mt-6 divide-y divide-zinc-200 rounded-lg border border-zinc-200 text-sm dark:divide-zinc-800 dark:border-zinc-800">
              {product.attributes.map((attribute) => (
                <div key={attribute.key} className="flex justify-between px-4 py-2">
                  <dt className="text-zinc-500">{attribute.key}</dt>
                  <dd className="font-medium text-zinc-900 dark:text-zinc-100">
                    {attribute.value}
                  </dd>
                </div>
              ))}
            </dl>
          )}

          <AddToCartButton product={product} />
        </div>
      </div>

      <ProductReviews productId={product.id} />
    </div>
  );
}
