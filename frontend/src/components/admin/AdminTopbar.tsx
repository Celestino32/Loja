import { LogoutButton } from "@/components/storefront/LogoutButton";
import type { AuthSession } from "@/lib/types";

export function AdminTopbar({ session }: { session: AuthSession }) {
  return (
    <header className="flex items-center justify-between border-b border-zinc-200 bg-white px-6 py-3 dark:border-zinc-800 dark:bg-zinc-950">
      <div />
      <div className="flex items-center gap-4 text-sm">
        <span className="text-zinc-600 dark:text-zinc-300">
          {session.fullName} <span className="text-zinc-400">· {session.roles.join(", ")}</span>
        </span>
        <LogoutButton className="font-medium text-red-600 hover:underline" />
      </div>
    </header>
  );
}
