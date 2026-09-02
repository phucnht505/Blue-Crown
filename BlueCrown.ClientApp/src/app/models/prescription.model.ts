export interface PrescriptionDispense {
  id: string;
  prescriptionItemId: string;
  productId: string;
  productName: string;
  quantityDispensed: number;
  dispensedBy: string | null;
  dispensedByName: string | null;
  dispensedAt: string | null;
}

export interface PrescriptionItem {
  id: string;
  prescriptionId: string;
  medicationId: string;
  medicationName: string;
  genericName: string | null;
  category: string | null;
  dosage: string | null;
  frequencyPerDay: number | null;
  durationDays: number | null;
  instructions: string | null;
  dispense: PrescriptionDispense | null;
}

export interface Prescription {
  id: string;
  appointmentId: string;
  medicalRecordId: string | null;
  medicalRecordDiagnosis: string;
  diagnosis: string;
  appointmentScheduledAt: string | null;
  appointmentType: string | null;
  patientId: string;
  patientName: string;
  doctorId: string;
  doctorName: string;
  doctorSpecialty: string;
  status: string | null;
  createdAt: string | null;
  items: PrescriptionItem[];
}

export interface CreatePrescriptionItemRequest {
  medicationId: string;
  dosage: string;
  frequencyPerDay: number | null;
  durationDays: number | null;
  instructions: string | null;
}

export interface CreatePrescriptionRequest {
  appointmentId?: string | null;
  medicalRecordId?: string | null;
  diagnosis?: string | null;
  items: CreatePrescriptionItemRequest[];
}

export interface DispensePrescriptionItemRequest {
  prescriptionItemId: string;
  productId: string;
  quantityDispensed: number;
}

export interface DispensePrescriptionRequest {
  items: DispensePrescriptionItemRequest[];
}
