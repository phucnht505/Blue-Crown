export interface CreateCheckoutItem {
  productId: string;
  quantity: number;
}

export interface CreateCheckoutRequest {
  guestPhone: string;
  shippingAddress: string;
  paymentMethod: string;
  prescriptionId: string | null;
  items: CreateCheckoutItem[];
}

export interface CheckoutItemResponse {
  id: string;
  productId: string;
  productName: string;
  productImageUrl: string | null;
  quantity: number;
  unitPrice: number;
}

export interface CheckoutResponse {
  id: string;
  userId: string | null;
  userName: string | null;
  guestPhone: string | null;
  shippingAddress: string;
  totalAmount: number;
  paymentMethod: string;
  paymentStatus: string;
  orderStatus: string;
  prescriptionId: string | null;
  createdAt: string | null;
  items: CheckoutItemResponse[];
}
