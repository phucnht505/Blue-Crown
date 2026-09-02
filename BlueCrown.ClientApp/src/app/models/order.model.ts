export interface OrderItem {
  id: string;
  productId: string;
  productName: string;
  productImageUrl: string | null;
  quantity: number;
  unitPrice: number;
}

export interface EcommerceOrder {
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
  items: OrderItem[];
}

export interface GuestOrderLookupRequest {
  orderId: string | null;
  guestPhone: string;
}
