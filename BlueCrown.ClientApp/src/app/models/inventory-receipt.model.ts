export interface ReceiptDetail {
  id: string;
  productId: string | null;
  productName: string | null;
  stockQuantity: number | null;
  batchNumber: string;
  expirationDate: string;
  quantityImported: number;
  importPrice: number;
}

export interface InventoryReceipt {
  id: string;
  supplierId: string | null;
  supplierName: string | null;
  createdBy: string | null;
  createdByName: string | null;
  approvedBy: string | null;
  approvedByName: string | null;
  totalCost: number | null;
  receiptDate: string | null;
  status: string | null;
  details: ReceiptDetail[];
}

export interface CreateReceiptDetailRequest {
  productId: string;
  batchNumber: string;
  expirationDate: string;
  quantityImported: number;
  importPrice: number;
}

export interface CreateInventoryReceiptRequest {
  supplierId: string;
  details: CreateReceiptDetailRequest[];
}
