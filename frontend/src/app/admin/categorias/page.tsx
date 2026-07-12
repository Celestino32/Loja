import { getAuthToken } from "@/lib/auth";
import { serverApiFetch } from "@/lib/api";
import type { CategoryDto } from "@/lib/types";
import { NewCategoryForm } from "@/components/admin/NewCategoryForm";
import { CategoryStatusToggle } from "@/components/admin/CategoryStatusToggle";

export default async function AdminCategoriasPage() {
  const token = await getAuthToken();
  const categories = await serverApiFetch<CategoryDto[]>("/api/categories?onlyActive=false", { token });

  return (
    <div>
      <h1 className="text-xl font-bold text-zinc-900 dark:text-white">Categorias</h1>

      <div className="mt-4 rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950">
        <NewCategoryForm />
      </div>

      <div className="mt-6 overflow-x-auto rounded-lg border border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-zinc-200 text-left text-xs uppercase tracking-wide text-zinc-400 dark:border-zinc-800">
              <th className="px-4 py-3">Nome</th>
              <th className="px-4 py-3">Slug</th>
              <th className="px-4 py-3">Descrição</th>
              <th className="px-4 py-3">Estado</th>
            </tr>
          </thead>
          <tbody>
            {categories.map((category) => (
              <tr key={category.id} className="border-b border-zinc-100 last:border-0 dark:border-zinc-900">
                <td className="px-4 py-3 font-medium text-zinc-900 dark:text-zinc-100">
                  {category.name}
                </td>
                <td className="px-4 py-3 text-zinc-500">{category.slug}</td>
                <td className="px-4 py-3 text-zinc-500">{category.description}</td>
                <td className="px-4 py-3">
                  <CategoryStatusToggle categoryId={category.id} isActive={category.isActive} />
                </td>
              </tr>
            ))}
            {categories.length === 0 && (
              <tr>
                <td colSpan={4} className="px-4 py-8 text-center text-zinc-400">
                  Nenhuma categoria cadastrada.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
