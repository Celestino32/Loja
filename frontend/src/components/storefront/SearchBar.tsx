import Form from "next/form";

export function SearchBar({ className }: { className?: string }) {
  return (
    <Form action="/produtos" className={`relative w-full ${className ?? ""}`}>
      <input
        type="text"
        name="search"
        placeholder="O que procuras?"
        className="w-full rounded-full border border-zinc-300 bg-white py-2.5 pl-4 pr-11 text-sm text-zinc-900 focus:border-blue-500 focus:outline-none dark:border-zinc-700 dark:bg-zinc-900 dark:text-white"
      />
      <button
        type="submit"
        aria-label="Buscar produtos"
        className="absolute right-1.5 top-1/2 flex h-8 w-8 -translate-y-1/2 items-center justify-center rounded-full bg-blue-600 text-white hover:bg-blue-500"
      >
        <svg viewBox="0 0 20 20" fill="none" className="h-4 w-4" aria-hidden="true">
          <path
            d="M9 16a7 7 0 1 1 0-14 7 7 0 0 1 0 14Zm8 2-3.6-3.6"
            stroke="currentColor"
            strokeWidth="1.7"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </button>
    </Form>
  );
}
