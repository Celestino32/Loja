"use client";

import { useRouter } from "next/navigation";
import { useTransition } from "react";

export function LogoutButton({ className }: { className?: string }) {
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  function handleLogout() {
    startTransition(async () => {
      await fetch("/api/auth/logout", { method: "POST" });
      router.refresh();
    });
  }

  return (
    <button type="button" onClick={handleLogout} disabled={isPending} className={className}>
      {isPending ? "A sair..." : "Sair"}
    </button>
  );
}
