import { serverApiFetch } from "@/lib/api";
import type { StoreSettingsDto } from "@/lib/types";

const FALLBACK: StoreSettingsDto = { storeName: "Loja Virtual", logoUrl: null };

export async function getStoreSettings(): Promise<StoreSettingsDto> {
  try {
    return await serverApiFetch<StoreSettingsDto>("/api/settings");
  } catch {
    return FALLBACK;
  }
}
