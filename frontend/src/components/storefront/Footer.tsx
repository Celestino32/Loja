import Image from "next/image";
import Link from "next/link";
import { serverApiFetch } from "@/lib/api";
import { resolveMediaUrl } from "@/lib/media";
import { getStoreSettings } from "@/lib/settings";
import type { CategoryDto } from "@/lib/types";

const WHATSAPP_NUMBER = process.env.NEXT_PUBLIC_WHATSAPP_NUMBER;

async function getCategories(): Promise<CategoryDto[]> {
  try {
    return await serverApiFetch<CategoryDto[]>("/api/categories?onlyActive=true");
  } catch {
    return [];
  }
}

export async function Footer() {
  const [settings, categories] = await Promise.all([getStoreSettings(), getCategories()]);
  const logoUrl = resolveMediaUrl(settings.logoUrl);

  return (
    <footer className="border-t border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
      <div className="mx-auto grid max-w-6xl gap-8 px-4 py-12 sm:px-6 md:grid-cols-4">
        <div>
          <div className="inline-block rounded-md bg-white p-1.5 shadow-sm">
            {logoUrl ? (
              <Image src={logoUrl} alt={settings.storeName} width={190} height={41} className="h-9 w-auto" unoptimized />
            ) : (
              <span className="block px-1 text-lg font-bold text-zinc-900">{settings.storeName}</span>
            )}
          </div>
          <p className="mt-3 text-sm text-zinc-500 dark:text-zinc-400">
            Produtos eletrónicos com garantia oficial e entrega em todas as províncias de Angola.
          </p>
        </div>

        <div>
          <p className="text-sm font-semibold text-zinc-900 dark:text-white">Categorias</p>
          <ul className="mt-3 space-y-1.5 text-sm text-zinc-500 dark:text-zinc-400">
            <li>
              <Link href="/produtos" className="hover:text-blue-600">
                Todos os produtos
              </Link>
            </li>
            {categories.map((category) => (
              <li key={category.id}>
                <Link href={`/produtos?categoryId=${category.id}`} className="hover:text-blue-600">
                  {category.name}
                </Link>
              </li>
            ))}
          </ul>
        </div>

        <div>
          <p className="text-sm font-semibold text-zinc-900 dark:text-white">Atendimento</p>
          <ul className="mt-3 space-y-1.5 text-sm text-zinc-500 dark:text-zinc-400">
            <li>cliente.online@storecelsjoscel.ao</li>
            {WHATSAPP_NUMBER && (
              <li>
                <a
                  href={`https://wa.me/${WHATSAPP_NUMBER}`}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="hover:text-blue-600"
                >
                  WhatsApp
                </a>
              </li>
            )}
            <li>
              <Link href="/minha-conta/pedidos" className="hover:text-blue-600">
                Acompanhar pedido
              </Link>
            </li>
            <li>
              <Link href="/registar" className="hover:text-blue-600">
                Criar conta
              </Link>
            </li>
          </ul>
        </div>

        <div>
          <p className="text-sm font-semibold text-zinc-900 dark:text-white">Pagamento &amp; Entrega</p>
          <ul className="mt-3 space-y-1.5 text-sm text-zinc-500 dark:text-zinc-400">
            <li>Multicaixa Express</li>
            <li>Referência de pagamento</li>
            <li>Transferência bancária</li>
            <li>Entrega por província/município</li>
            <li>Retirada na loja (Click &amp; Collect)</li>
          </ul>
        </div>
      </div>

      <div className="border-t border-zinc-200 py-4 text-center text-xs text-zinc-400 dark:border-zinc-800">
        © {new Date().getFullYear()} {settings.storeName} — Todos os direitos reservados.
      </div>
    </footer>
  );
}
