const PUBLIC_API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5050";

/** Uploads (produtos, logo) voltam com URL relativa ("/uploads/...") ao host da API,
 * não ao frontend — precisa ser resolvida para um URL absoluto antes de renderizar. */
export function resolveMediaUrl(url: string | null | undefined): string | null {
  if (!url) return null;
  if (url.startsWith("http://") || url.startsWith("https://")) return url;
  return `${PUBLIC_API_URL}${url}`;
}
