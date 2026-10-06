export type StatisticsPeriod = 'day' | 'month' | 'year';

export interface SalesOrderStatistic {
  id: string;
  createdAt: string | null;
  customerName: string;
  guestPhone: string | null;
  totalAmount: number;
  paymentMethod: string;
  paymentStatus: string;
  orderStatus: string;
}

export interface PrescriptionDispenseStatistic {
  id: string;
  productName: string;
  quantityDispensed: number;
  unitPrice: number;
  dispensedByName: string | null;
  dispensedAt: string | null;
  totalAmount: number;
}

export interface InventoryReceiptStatistic {
  id: string;
  receiptDate: string | null;
  supplierName: string;
  totalCost: number;
  status: string;
}

export interface AdminStatistics {
  period: StatisticsPeriod;
  fromDate: string;
  toDate: string;

  salesOrderCount: number;
  salesRevenue: number;

  dispensedItemCount: number;
  prescriptionRevenue: number;
  dispensedItems: PrescriptionDispenseStatistic[];

  totalRevenue: number;

  inventoryReceiptCount: number;
  inventoryCost: number;

  balance: number;

  salesOrders: SalesOrderStatistic[];
  inventoryReceipts: InventoryReceiptStatistic[];
}
