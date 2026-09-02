export interface MedicalRecord {
  id: string;
  appointmentId: string | null;
  patientId: string;
  patientName: string;
  doctorId: string;
  doctorName: string;
  doctorSpecialty: string;
  appointmentScheduledAt: string | null;
  appointmentType: string | null;
  diagnosis: string;
  notes: string | null;
  createdAt: string | null;
}

export interface CreateMedicalRecordRequest {
  appointmentId: string;
  diagnosis: string;
  notes: string | null;
}

export interface UpdateMedicalRecordRequest {
  diagnosis: string;
  notes: string | null;
}
