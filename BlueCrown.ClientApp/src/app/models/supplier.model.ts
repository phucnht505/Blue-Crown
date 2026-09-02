export interface Supplier {
  id: string;
  supplierName: string;
  contactPhone: string;
  gdpCertified: boolean | null;
  createdAt: string | null;
}

export interface CreateSupplierRequest {
  supplierName: string;
  contactPhone: string;
  gdpCertified: boolean | null;
}

export interface UpdateSupplierRequest {
  supplierName: string;
  contactPhone: string;
  gdpCertified: boolean | null;
}
