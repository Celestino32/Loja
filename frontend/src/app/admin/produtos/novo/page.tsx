import { getAuthToken } from "@/lib/auth";
import { serverApiFetch } from "@/lib/api";
import type { CategoryDto } from "@/lib/types";
import { ProductForm } from "@/components/admin/ProductForm";

export default async function NovoProdutoPage() {
  const token = await getAuthToken();
  const categories = await serverApiFetch<CategoryDto[]>("/api/categories?onlyActive=false", { token });

  return (
    <div>
      <h1 className="text-xl font-bold text-zinc-900 dark:text-white">Novo produto</h1>
      <ProductForm categories={categories} />
    </div>
  );
}
