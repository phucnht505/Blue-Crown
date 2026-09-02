export interface Product {
  id: string;
  medicationId: string | null;
  medicationName: string | null;
  medicationGenericName: string | null;
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number | null;
  isPrescriptionRequired: boolean | null;
  activeIngredient: string | null;
  therapeuticGroup: string | null;
  dosageForm: string | null;
  strength: string | null;
  imageUrl: string | null;
}

export interface ProductRequest {
  medicationId: string | null;
  name: string;
  description: string | null;
  price: number;
  isPrescriptionRequired: boolean;
  activeIngredient: string | null;
  therapeuticGroup: string | null;
  dosageForm: string | null;
  strength: string | null;
  imageUrl: string | null;
}

export interface DrugAlternative {
  id: string;
  productId: string;
  productName: string;
  alternativeProductId: string;
  alternativeProductName: string;
  reason: string | null;
  similarityScore: number | null;
  createdAt: string | null;
}
