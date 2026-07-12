export interface CategoryDto {
  id: string;
  name: string;
  slug: string;
  description: string | null;
  isActive: boolean;
}

export interface ProductAttributeDto {
  key: string;
  value: string;
}

export interface ProductImageDto {
  id: string;
  url: string;
  sortOrder: number;
}

export interface StoreSettingsDto {
  storeName: string;
  logoUrl: string | null;
}

export interface ProductListItemDto {
  id: string;
  sku: string;
  name: string;
  slug: string;
  brand: string;
  price: number;
  stockQuantity: number;
  isActive: boolean;
  categoryId: string;
  categoryName: string | null;
  primaryImageUrl: string | null;
}

export interface ProductDto {
  id: string;
  sku: string;
  name: string;
  slug: string;
  description: string | null;
  brand: string;
  price: number;
  stockQuantity: number;
  isActive: boolean;
  categoryId: string;
  categoryName: string | null;
  attributes: ProductAttributeDto[];
  images: ProductImageDto[];
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export type StaffRole = "Admin" | "Gerente" | "Vendedor";

export interface AuthSession {
  fullName: string;
  email: string;
  roles: StaffRole[];
}

export interface ShippingZoneDto {
  id: string;
  province: string;
  municipality: string;
  cost: number;
  isActive: boolean;
}

export interface CustomerAddressDto {
  id: string;
  province: string;
  municipality: string;
  street: string;
  reference: string | null;
  isDefault: boolean;
}

export interface CustomerDto {
  id: string;
  fullName: string;
  phone: string;
  nif: string | null;
  addresses: CustomerAddressDto[];
}

export type FulfillmentType = "Entrega" | "RetiradaNaLoja";

export type PaymentMethod = "MulticaixaExpress" | "ReferenciaDePagamento" | "TransferenciaBancaria";

export type PaymentStatus = "Pendente" | "Confirmado" | "Falhado";

export type OrderStatus =
  | "Pendente"
  | "Pago"
  | "Preparando"
  | "ProntoParaEntregaOuRetirada"
  | "Concluido"
  | "Cancelado";

export interface CheckoutItemRequest {
  productId: string;
  quantity: number;
}

export interface CheckoutRequest {
  items: CheckoutItemRequest[];
  fulfillmentType: FulfillmentType;
  shippingZoneId: string | null;
  shippingStreet: string | null;
  shippingReference: string | null;
  paymentMethod: PaymentMethod;
  couponCode?: string | null;
}

export interface CheckoutResultDto {
  orderId: string;
  orderNumber: string;
  total: number;
  discountTotal: number;
  paymentMethod: PaymentMethod;
  paymentExternalReference: string | null;
  paymentInstructions: string;
}

export interface OrderSummaryDto {
  id: string;
  orderNumber: string;
  status: OrderStatus;
  total: number;
  createdAtUtc: string;
}

export interface OrderItemDto {
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface PaymentDto {
  method: PaymentMethod;
  status: PaymentStatus;
  amount: number;
  externalReference: string | null;
  instructions: string | null;
  confirmedAtUtc: string | null;
}

export interface OrderDto {
  id: string;
  orderNumber: string;
  status: OrderStatus;
  fulfillmentType: FulfillmentType;
  shippingProvince: string | null;
  shippingMunicipality: string | null;
  shippingStreet: string | null;
  shippingReference: string | null;
  shippingCost: number;
  discountTotal: number;
  total: number;
  createdAtUtc: string;
  items: OrderItemDto[];
  payment: PaymentDto | null;
  hasInvoice: boolean;
  customerName: string;
  customerPhone: string;
}

export interface ReviewDto {
  id: string;
  customerName: string;
  rating: number;
  comment: string | null;
  createdAtUtc: string;
}

export interface ProductReviewsDto {
  averageRating: number;
  totalCount: number;
  items: ReviewDto[];
}

export type CouponDiscountType = "Percentual" | "ValorFixo";

export interface CouponDto {
  id: string;
  code: string;
  discountType: CouponDiscountType;
  discountValue: number;
  minOrderValue: number | null;
  maxUses: number | null;
  usedCount: number;
  validFrom: string | null;
  validUntil: string | null;
  isActive: boolean;
}

export interface CouponValidationResultDto {
  isValid: boolean;
  discountAmount: number;
  message: string | null;
}

export interface DailySalesDto {
  date: string;
  revenue: number;
  orderCount: number;
}

export interface SalesReportDto {
  totalRevenue: number;
  orderCount: number;
  averageOrderValue: number;
  dailyBreakdown: DailySalesDto[];
}

export interface TopProductDto {
  productId: string;
  productName: string;
  quantitySold: number;
  revenue: number;
}

export interface LowStockProductDto {
  productId: string;
  productName: string;
  sku: string;
  stockQuantity: number;
}
