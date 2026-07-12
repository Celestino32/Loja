import { redirect } from "next/navigation";
import { getServerSession, hasStaffRole } from "@/lib/auth";
import { getStoreSettings } from "@/lib/settings";
import { AdminSidebar } from "@/components/admin/AdminSidebar";
import { AdminTopbar } from "@/components/admin/AdminTopbar";

export default async function AdminLayout({ children }: { children: React.ReactNode }) {
  const session = await getServerSession();

  if (!hasStaffRole(session)) {
    redirect("/login?redirectTo=/admin");
  }

  const settings = await getStoreSettings();

  return (
    <div className="flex min-h-full">
      <AdminSidebar roles={session!.roles} settings={settings} />
      <div className="flex flex-1 flex-col">
        <AdminTopbar session={session!} />
        <main className="flex-1 bg-zinc-50 p-6 dark:bg-zinc-900">{children}</main>
      </div>
    </div>
  );
}
