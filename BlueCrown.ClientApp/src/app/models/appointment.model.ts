export interface AppointmentDoctor {
  id: string;
  fullName: string;
  specialty: string;
  clinicName: string | null;
  yearsExperience: number | null;
  consultationFee: number | null;
  ratingAvg: number | null;
}

export interface Appointment {
  id: string;
  patientId: string;
  patientName: string;
  doctorId: string;
  doctorName: string;
  doctorSpecialty: string;
  clinicName: string | null;
  consultationFee: number | null;
  scheduledAt: string;
  type: string | null;
  status: string | null;
  createdAt: string | null;
}

export interface CreateAppointmentRequest {
  doctorId: string;
  scheduledAt: string;
  type: string;
}

export interface AppointmentMessage {
  message: string;
}
