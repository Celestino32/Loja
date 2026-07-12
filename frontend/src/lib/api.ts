export const PUBLIC_API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5050";
const INTERNAL_API_URL = process.env.API_INTERNAL_URL ?? PUBLIC_API_URL;

export class ApiError extends Error {
  constructor(
    public status: number,
    message: string,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

async function parseError(response: Response): Promise<string> {
  try {
    const body = await response.json();
    return body.detail ?? body.title ?? body.message ?? response.statusText;
  } catch {
    return response.statusText;
  }
}

/** Chamadas feitas a partir do browser (rotas públicas: catálogo, categorias). */
export async function publicApiFetch<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${PUBLIC_API_URL}${path}`, {
    ...init,
    headers: { "Content-Type": "application/json", ...init?.headers },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new ApiError(response.status, await parseError(response));
  }

  if (response.status === 204) return undefined as T;
  return (await response.json()) as T;
}

/** Chamadas feitas em Server Components/Route Handlers, opcionalmente com token de autenticação. */
export async function serverApiFetch<T>(
  path: string,
  init?: RequestInit & { token?: string },
): Promise<T> {
  const { token, ...rest } = init ?? {};

  const response = await fetch(`${INTERNAL_API_URL}${path}`, {
    ...rest,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...rest?.headers,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new ApiError(response.status, await parseError(response));
  }

  if (response.status === 204) return undefined as T;
  return (await response.json()) as T;
}

/** Upload multipart (imagens) a partir de Server Components/Server Actions, com token. */
export async function serverApiUpload<T>(path: string, formData: FormData, token?: string): Promise<T> {
  const response = await fetch(`${INTERNAL_API_URL}${path}`, {
    method: "POST",
    body: formData,
    headers: token ? { Authorization: `Bearer ${token}` } : undefined,
    cache: "no-store",
  });

  if (!response.ok) {
    throw new ApiError(response.status, await parseError(response));
  }

  if (response.status === 204) return undefined as T;
  return (await response.json()) as T;
}
