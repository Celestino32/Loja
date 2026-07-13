import Link from "next/link";
import { serverApiFetch } from "@/lib/api";
import { getServerSession, hasStaffRole } from "@/lib/auth";
import type { CategoryDto } from "@/lib/types";
import { CartBadge } from "./CartBadge";
import { LogoutButton } from "./LogoutButton";
import { SearchBar } from "./SearchBar";

async function getCategories(): Promise<CategoryDto[]> {
  try {
    return await serverApiFetch<CategoryDto[]>("/api/categories");
  } catch {
    return [];
  }
}

export async function Header() {
  const [categories, session] = await Promise.all([
    getCategories(),
    getServerSession(),
  ]);

  return (
    <header className="sticky top-0 z-30 bg-white dark:bg-zinc-950">
      {/* Barra principal: logo, busca, conta */}
      <div className="border-b border-zinc-200 dark:border-zinc-800">
        <div className="mx-auto flex max-w-6xl flex-wrap items-center gap-3 px-4 py-3 sm:flex-nowrap sm:gap-6 sm:px-6">
          <Link href="/" className="order-1 shrink-0 rounded-md bg-white px-2 py-1.5 shadow-sm">
            <span className="block px-1 text-xl font-bold tracking-tight text-zinc-900">CELSTORE</span>
          </Link>

          <div className="order-3 w-full sm:order-2 sm:max-w-xl sm:flex-1">
            <SearchBar />
          </div>

          <div className="order-2 ml-auto flex shrink-0 items-center gap-3 sm:order-3 sm:ml-0">
            <div className="hidden items-center gap-1.5 text-xs text-zinc-500 md:flex dark:text-zinc-400">
              <svg viewBox="0 0 20 20" fill="none" className="h-4 w-4 shrink-0" aria-hidden="true">
                <path
                  d="M10 18s6-5.1 6-9.7A6 6 0 0 0 4 8.3C4 12.9 10 18 10 18Z"
                  stroke="currentColor"
                  strokeWidth="1.4"
                  strokeLinejoin="round"
                />
                <circle cx="10" cy="8.3" r="2" stroke="currentColor" strokeWidth="1.4" />
              </svg>
              <span>Entregamos em toda Angola</span>
            </div>

            <CartBadge />

            {hasStaffRole(session) ? (
              <Link
                href="/admin"
                className="rounded-md bg-zinc-900 px-4 py-2 text-sm font-medium text-white hover:bg-zinc-700 dark:bg-white dark:text-zinc-900"
              >
                Painel ({session!.fullName.split(" ")[0]})
              </Link>
            ) : session ? (
              <div className="flex items-center gap-3 text-sm text-zinc-600 dark:text-zinc-300">
                <Link href="/minha-conta/pedidos" className="hidden hover:text-blue-600 sm:inline">
                  Meus pedidos
                </Link>
                <span className="hidden sm:inline">Olá, {session.fullName.split(" ")[0]}</span>
                <LogoutButton className="font-medium text-blue-600 hover:underline" />
              </div>
            ) : (
              <Link
                href="/login"
                className="rounded-md border border-zinc-300 px-4 py-2 text-sm font-medium text-zinc-700 hover:border-zinc-400 dark:border-zinc-700 dark:text-zinc-200"
              >
                Entrar
              </Link>
            )}
          </div>
        </div>
      </div>

      {/* Barra de categorias */}
      <nav className="border-b border-zinc-200 bg-zinc-50 dark:border-zinc-800 dark:bg-zinc-900">
        <div className="mx-auto flex max-w-6xl gap-6 overflow-x-auto px-4 py-2.5 text-sm font-medium text-zinc-600 sm:px-6 dark:text-zinc-300">
          <Link href="/produtos" className="shrink-0 whitespace-nowrap hover:text-blue-600">
            Todos os produtos
          </Link>
          {categories.map((category) => (
            <Link
              key={category.id}
              href={`/produtos?categoryId=${category.id}`}
              className="shrink-0 whitespace-nowrap hover:text-blue-600"
            >
              {category.name}
            </Link>
          ))}
        </div>
      </nav>
    </header>
  );
}
