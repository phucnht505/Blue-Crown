export interface PatientProfile {
  id: string;
  userId: string;
  bloodType: string;
  heightCm: number | null;
  weightKg: number | null;
  allergies: string | null;
  chronicConditions: string | null;
  emergencyContactName: string | null;
  emergencyContactPhone: string | null;
}

export interface PatientProfileRequest {
  bloodType: string;
  heightCm: number;
  weightKg: number;
  allergies: string | null;
  chronicConditions: string | null;
  emergencyContactName: string | null;
  emergencyContactPhone: string | null;
}

export interface PatientProfileMessage {
  message: string;
}
