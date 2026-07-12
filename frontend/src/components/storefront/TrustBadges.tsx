const badges = [
  {
    title: "Multicaixa",
    description: "Express ou Referência",
    icon: (
      <path
        d="M4 8a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V8Z M4 10h16"
        stroke="currentColor"
        strokeWidth="1.6"
        strokeLinejoin="round"
      />
    ),
  },
  {
    title: "Suporte online",
    description: "atendimento@storecelsjoscel.ao",
    icon: (
      <path
        d="M4 6h16v10H8l-4 4V6Z"
        stroke="currentColor"
        strokeWidth="1.6"
        strokeLinejoin="round"
      />
    ),
  },
  {
    title: "Envio Nacional",
    description: "Entregas seguras em dias úteis",
    icon: (
      <path
        d="M3 7h11v9H3V7Zm11 3h4l3 3v3h-7v-6Z M6.5 19a1.5 1.5 0 1 0 0-3 1.5 1.5 0 0 0 0 3Zm11 0a1.5 1.5 0 1 0 0-3 1.5 1.5 0 0 0 0 3Z"
        stroke="currentColor"
        strokeWidth="1.4"
        strokeLinejoin="round"
      />
    ),
  },
  {
    title: "Entrega grátis em Luanda",
    description: "Dias úteis das 9h às 17h",
    icon: (
      <path
        d="M12 3v12m0 0-4-4m4 4 4-4M5 19h14"
        stroke="currentColor"
        strokeWidth="1.6"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    ),
  },
];

export function TrustBadges() {
  return (
    <section className="border-b border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
      <div className="mx-auto grid max-w-6xl grid-cols-2 gap-6 px-4 py-8 sm:px-6 md:grid-cols-4">
        {badges.map((badge) => (
          <div key={badge.title} className="flex items-start gap-3">
            <svg viewBox="0 0 24 24" fill="none" className="mt-0.5 h-7 w-7 shrink-0 text-blue-600" aria-hidden="true">
              {badge.icon}
            </svg>
            <div>
              <p className="text-sm font-semibold text-zinc-900 dark:text-white">{badge.title}</p>
              <p className="text-xs text-zinc-500 dark:text-zinc-400">{badge.description}</p>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}
