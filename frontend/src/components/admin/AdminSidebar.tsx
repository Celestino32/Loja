import Image from "next/image";
import Link from "next/link";
import { resolveMediaUrl } from "@/lib/media";
import type { StaffRole, StoreSettingsDto } from "@/lib/types";

const links: { href: string; label: string; roles?: StaffRole[] }[] = [
  { href: "/admin", label: "Dashboard" },
  { href: "/admin/pedidos", label: "Pedidos" },
  { href: "/admin/produtos", label: "Produtos" },
  { href: "/admin/categorias", label: "Categorias" },
  { href: "/admin/frete", label: "Zonas de frete", roles: ["Admin", "Gerente"] },
  { href: "/admin/cupons", label: "Cupons", roles: ["Admin", "Gerente"] },
  { href: "/admin/relatorios", label: "Relatórios", roles: ["Admin", "Gerente"] },
  { href: "/admin/funcionarios", label: "Funcionários", roles: ["Admin"] },
  { href: "/admin/configuracoes", label: "Configurações", roles: ["Admin"] },
];

export function AdminSidebar({ roles, settings }: { roles: StaffRole[]; settings: StoreSettingsDto }) {
  const logoUrl = resolveMediaUrl(settings.logoUrl);

  return (
    <aside className="w-56 shrink-0 border-r border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
      <div className="px-4 py-5">
        <Link href="/" className="inline-block rounded-md bg-white p-1 shadow-sm">
          {logoUrl ? (
            <Image src={logoUrl} alt={settings.storeName} width={170} height={36} className="h-8 w-auto" unoptimized />
          ) : (
            <span className="block px-1 text-base font-bold text-zinc-900">{settings.storeName}</span>
          )}
        </Link>
        <p className="mt-1 text-xs text-zinc-400">Painel administrativo</p>
      </div>

      <nav className="flex flex-col gap-1 px-2">
        {links
          .filter((link) => !link.roles || link.roles.some((role) => roles.includes(role)))
          .map((link) => (
            <Link
              key={link.href}
              href={link.href}
              className="rounded-md px-3 py-2 text-sm font-medium text-zinc-600 hover:bg-zinc-100 hover:text-zinc-900 dark:text-zinc-300 dark:hover:bg-zinc-900 dark:hover:text-white"
            >
              {link.label}
            </Link>
          ))}
      </nav>
    </aside>
  );
}
