const WHATSAPP_NUMBER = process.env.NEXT_PUBLIC_WHATSAPP_NUMBER;

export function WhatsAppButton() {
  if (!WHATSAPP_NUMBER) return null;

  return (
    <a
      href={`https://wa.me/${WHATSAPP_NUMBER}`}
      target="_blank"
      rel="noopener noreferrer"
      aria-label="Falar no WhatsApp"
      className="fixed bottom-5 right-5 z-40 flex h-14 w-14 items-center justify-center rounded-full bg-emerald-500 text-white shadow-lg transition-transform hover:scale-105 hover:bg-emerald-400"
    >
      <svg viewBox="0 0 24 24" fill="currentColor" className="h-7 w-7" aria-hidden="true">
        <path d="M12.01 2C6.5 2 2.02 6.48 2.02 12c0 1.77.46 3.45 1.28 4.9L2 22l5.25-1.28A9.96 9.96 0 0 0 12.01 22C17.52 22 22 17.52 22 12S17.52 2 12.01 2Zm5.4 14.2c-.23.65-1.34 1.24-1.85 1.3-.5.06-1 .27-3.26-.68-2.75-1.16-4.5-3.98-4.64-4.16-.14-.19-1.1-1.47-1.1-2.8 0-1.34.7-2 .95-2.27.25-.27.55-.34.73-.34.18 0 .37 0 .53.01.17.01.4-.06.62.48.23.55.78 1.9.85 2.04.07.14.11.3.02.49-.09.19-.14.3-.27.46-.14.16-.29.36-.41.48-.14.14-.28.29-.12.57.16.28.71 1.18 1.53 1.91 1.05.94 1.94 1.24 2.22 1.38.28.14.44.12.6-.07.16-.19.7-.82.89-1.1.19-.28.37-.23.62-.14.25.09 1.6.76 1.87.9.28.14.46.21.53.32.07.12.07.65-.16 1.3Z" />
      </svg>
    </a>
  );
}
