export interface Medication {
  id: string;
  name: string;
  genericName: string | null;
  category: string | null;
}

export interface CreateMedicationRequest {
  name: string;
  genericName: string | null;
  category: string | null;
}

export interface UpdateMedicationRequest {
  name: string;
  genericName: string | null;
  category: string | null;
}
