import { redirect } from "next/navigation";
import { getServerSession } from "@/lib/auth";
import { getStoreSettings } from "@/lib/settings";
import { LogoUploadForm } from "@/components/admin/LogoUploadForm";

export default async function AdminConfiguracoesPage() {
  const session = await getServerSession();

  if (!session?.roles.includes("Admin")) {
    redirect("/admin");
  }

  const settings = await getStoreSettings();

  return (
    <div>
      <h1 className="text-xl font-bold text-zinc-900 dark:text-white">Configurações</h1>
      <p className="mt-1 text-sm text-zinc-500">Identidade visual da loja, usada no site e no painel.</p>

      <div className="mt-6 rounded-lg border border-zinc-200 bg-white p-6 dark:border-zinc-800 dark:bg-zinc-950">
        <h2 className="mb-3 font-semibold text-zinc-900 dark:text-white">Logotipo</h2>
        <LogoUploadForm storeName={settings.storeName} logoUrl={settings.logoUrl} />
      </div>
    </div>
  );
}
