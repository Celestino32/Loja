"use server";

import { redirect } from "next/navigation";
import { revalidatePath } from "next/cache";
import { serverApiFetch, serverApiUpload } from "@/lib/api";
import { getAuthToken, getServerSession, hasStaffRole } from "@/lib/auth";
import type { ProductImageDto } from "@/lib/types";

async function requireStaffToken(): Promise<string> {
  const [session, token] = await Promise.all([getServerSession(), getAuthToken()]);
  if (!hasStaffRole(session) || !token) {
    throw new Error("Não autorizado.");
  }
  return token;
}

async function requireAdminToken(): Promise<string> {
  const [session, token] = await Promise.all([getServerSession(), getAuthToken()]);
  if (!session?.roles.includes("Admin") || !token) {
    throw new Error("Apenas o administrador pode executar esta ação.");
  }
  return token;
}

export async function createProduct(formData: FormData) {
  const token = await requireStaffToken();

  const attributes: { key: string; value: string }[] = [];
  const attrKeys = formData.getAll("attributeKey") as string[];
  const attrValues = formData.getAll("attributeValue") as string[];
  attrKeys.forEach((key, i) => {
    if (key.trim() && attrValues[i]?.trim()) {
      attributes.push({ key: key.trim(), value: attrValues[i].trim() });
    }
  });

  const id = await serverApiFetch<string>("/api/products", {
    method: "POST",
    token,
    body: JSON.stringify({
      sku: formData.get("sku"),
      name: formData.get("name"),
      slug: formData.get("slug"),
      brand: formData.get("brand"),
      price: Number(formData.get("price")),
      stockQuantity: Number(formData.get("stockQuantity")),
      categoryId: formData.get("categoryId"),
      description: (formData.get("description") as string) || null,
      attributes,
      imageUrls: [],
    }),
  });

  revalidatePath("/admin/produtos");
  redirect(`/admin/produtos/${id}`);
}

export async function updateProduct(id: string, formData: FormData) {
  const token = await requireStaffToken();

  await serverApiFetch(`/api/products/${id}`, {
    method: "PUT",
    token,
    body: JSON.stringify({
      id,
      name: formData.get("name"),
      slug: formData.get("slug"),
      brand: formData.get("brand"),
      description: (formData.get("description") as string) || null,
      categoryId: formData.get("categoryId"),
      price: Number(formData.get("price")),
    }),
  });

  revalidatePath("/admin/produtos");
  revalidatePath(`/admin/produtos/${id}`);
}

export async function setProductActive(id: string, isActive: boolean) {
  const token = await requireStaffToken();

  await serverApiFetch(`/api/products/${id}/status`, {
    method: "PATCH",
    token,
    body: JSON.stringify(isActive),
  });

  revalidatePath("/admin/produtos");
  revalidatePath(`/admin/produtos/${id}`);
}

export async function adjustStock(id: string, quantity: number) {
  const token = await requireStaffToken();

  await serverApiFetch(`/api/products/${id}/stock`, {
    method: "PATCH",
    token,
    body: JSON.stringify(quantity),
  });

  revalidatePath("/admin/produtos");
  revalidatePath(`/admin/produtos/${id}`);
}

export async function uploadProductImage(productId: string, formData: FormData) {
  const token = await requireStaffToken();

  const file = formData.get("file") as File | null;
  if (!file || file.size === 0) {
    throw new Error("Selecione um arquivo de imagem.");
  }

  const uploadData = new FormData();
  uploadData.set("file", file);

  const image = await serverApiUpload<ProductImageDto>(`/api/products/${productId}/images`, uploadData, token);

  revalidatePath(`/admin/produtos/${productId}`);
  return image;
}

export async function removeProductImage(productId: string, imageId: string) {
  const token = await requireStaffToken();
  await serverApiFetch(`/api/products/${productId}/images/${imageId}`, { method: "DELETE", token });
  revalidatePath(`/admin/produtos/${productId}`);
}

export async function uploadStoreLogo(formData: FormData) {
  const token = await requireAdminToken();

  const file = formData.get("file") as File | null;
  if (!file || file.size === 0) {
    throw new Error("Selecione um arquivo de imagem.");
  }

  const uploadData = new FormData();
  uploadData.set("file", file);

  await serverApiUpload("/api/settings/logo", uploadData, token);

  revalidatePath("/admin", "layout");
  revalidatePath("/", "layout");
}

export async function createCategory(formData: FormData) {
  const token = await requireStaffToken();

  await serverApiFetch("/api/categories", {
    method: "POST",
    token,
    body: JSON.stringify({
      name: formData.get("name"),
      slug: formData.get("slug"),
      description: (formData.get("description") as string) || null,
    }),
  });

  revalidatePath("/admin/categorias");
}

export async function setCategoryActive(id: string, isActive: boolean) {
  const token = await requireStaffToken();

  await serverApiFetch(`/api/categories/${id}/status`, {
    method: "PATCH",
    token,
    body: JSON.stringify(isActive),
  });

  revalidatePath("/admin/categorias");
}

export async function createShippingZone(formData: FormData) {
  const token = await requireStaffToken();

  await serverApiFetch("/api/shipping-zones", {
    method: "POST",
    token,
    body: JSON.stringify({
      province: formData.get("province"),
      municipality: formData.get("municipality"),
      cost: Number(formData.get("cost")),
    }),
  });

  revalidatePath("/admin/frete");
}

export async function setShippingZoneActive(id: string, isActive: boolean) {
  const token = await requireStaffToken();

  await serverApiFetch(`/api/shipping-zones/${id}/status`, {
    method: "PATCH",
    token,
    body: JSON.stringify(isActive),
  });

  revalidatePath("/admin/frete");
}

export async function confirmOrderPayment(id: string) {
  const token = await requireStaffToken();
  await serverApiFetch(`/api/orders/${id}/confirm-payment`, { method: "PATCH", token });
  revalidatePath("/admin/pedidos");
  revalidatePath(`/admin/pedidos/${id}`);
}

export async function startPreparingOrder(id: string) {
  const token = await requireStaffToken();
  await serverApiFetch(`/api/orders/${id}/start-preparing`, { method: "PATCH", token });
  revalidatePath("/admin/pedidos");
  revalidatePath(`/admin/pedidos/${id}`);
}

export async function markOrderReady(id: string) {
  const token = await requireStaffToken();
  await serverApiFetch(`/api/orders/${id}/mark-ready`, { method: "PATCH", token });
  revalidatePath("/admin/pedidos");
  revalidatePath(`/admin/pedidos/${id}`);
}

export async function completeOrder(id: string) {
  const token = await requireStaffToken();
  await serverApiFetch(`/api/orders/${id}/complete`, { method: "PATCH", token });
  revalidatePath("/admin/pedidos");
  revalidatePath(`/admin/pedidos/${id}`);
}

export async function cancelOrder(id: string) {
  const token = await requireStaffToken();
  await serverApiFetch(`/api/orders/${id}/cancel`, { method: "PATCH", token });
  revalidatePath("/admin/pedidos");
  revalidatePath(`/admin/pedidos/${id}`);
}

export async function createCoupon(formData: FormData) {
  const token = await requireStaffToken();

  const minOrderValue = formData.get("minOrderValue") as string;
  const maxUses = formData.get("maxUses") as string;
  const validFrom = formData.get("validFrom") as string;
  const validUntil = formData.get("validUntil") as string;

  await serverApiFetch("/api/coupons", {
    method: "POST",
    token,
    body: JSON.stringify({
      code: formData.get("code"),
      discountType: formData.get("discountType"),
      discountValue: Number(formData.get("discountValue")),
      minOrderValue: minOrderValue ? Number(minOrderValue) : null,
      maxUses: maxUses ? Number(maxUses) : null,
      validFrom: validFrom || null,
      validUntil: validUntil || null,
    }),
  });

  revalidatePath("/admin/cupons");
}

export async function setCouponActive(id: string, isActive: boolean) {
  const token = await requireStaffToken();

  await serverApiFetch(`/api/coupons/${id}/status`, {
    method: "PATCH",
    token,
    body: JSON.stringify(isActive),
  });

  revalidatePath("/admin/cupons");
}

export async function createStaff(formData: FormData) {
  const token = await requireAdminToken();

  await serverApiFetch("/api/auth/staff", {
    method: "POST",
    token,
    body: JSON.stringify({
      fullName: formData.get("fullName"),
      email: formData.get("email"),
      password: formData.get("password"),
      role: formData.get("role"),
    }),
  });

  revalidatePath("/admin/funcionarios");
}
