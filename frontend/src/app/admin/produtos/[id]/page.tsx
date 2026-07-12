import { notFound } from "next/navigation";
import { getAuthToken } from "@/lib/auth";
import { ApiError, serverApiFetch } from "@/lib/api";
import type { CategoryDto, ProductDto } from "@/lib/types";
import { ProductForm } from "@/components/admin/ProductForm";
import { ProductImageManager } from "@/components/admin/ProductImageManager";
import { StockAdjuster } from "@/components/admin/StockAdjuster";
import { ProductStatusToggle } from "@/components/admin/ProductStatusToggle";

interface EditProdutoPageProps {
  params: Promise<{ id: string }>;
}

export default async function EditProdutoPage({ params }: EditProdutoPageProps) {
  const { id } = await params;
  const token = await getAuthToken();

  let product: ProductDto;
  try {
    product = await serverApiFetch<ProductDto>(`/api/products/${id}`, { token });
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) notFound();
    throw error;
  }

  const categories = await serverApiFetch<CategoryDto[]>("/api/categories?onlyActive=false", { token });

  return (
    <div>
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-bold text-zinc-900 dark:text-white">{product.name}</h1>
        <ProductStatusToggle productId={product.id} isActive={product.isActive} />
      </div>
      <p className="text-sm text-zinc-500">SKU: {product.sku}</p>

      <div className="mt-6 rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <StockAdjuster productId={product.id} stockQuantity={product.stockQuantity} />
      </div>

      <div className="mt-6 rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <h2 className="mb-3 font-semibold text-zinc-900 dark:text-white">Fotos</h2>
        <ProductImageManager productId={product.id} images={product.images} />
      </div>

      <ProductForm categories={categories} product={product} />
    </div>
  );
}
