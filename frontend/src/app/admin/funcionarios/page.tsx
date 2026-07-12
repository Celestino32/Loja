import { redirect } from "next/navigation";
import { getServerSession } from "@/lib/auth";
import { NewStaffForm } from "@/components/admin/NewStaffForm";

export default async function AdminFuncionariosPage() {
  const session = await getServerSession();

  if (!session?.roles.includes("Admin")) {
    redirect("/admin");
  }

  return (
    <div>
      <h1 className="text-xl font-bold text-zinc-900 dark:text-white">Funcionários</h1>
      <p className="mt-1 text-sm text-zinc-500">
        Apenas o administrador pode criar contas de Gerente e Vendedor. Não há autocadastro de
        equipa.
      </p>

      <div className="mt-6 rounded-lg border border-zinc-200 bg-white p-6 dark:border-zinc-800 dark:bg-zinc-950">
        <NewStaffForm />
      </div>
    </div>
  );
}
