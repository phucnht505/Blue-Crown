export interface HealthGoal {
  id: string;
  patientId: string;
  metricTypeId: number;
  metricTypeCode: string;
  metricTypeName: string;
  metricTypeUnit: string;
  targetValue: number | null;
  startDate: string | null;
  endDate: string | null;
  status: string | null;
  createdByUserId: string;
  createdByRole: string;
}

export interface CreateHealthGoalRequest {
  metricTypeId: number;
  targetValue: number;
  startDate: string | null;
  endDate: string | null;
}

export interface UpdateHealthGoalRequest {
  metricTypeId: number;
  targetValue: number;
  startDate: string | null;
  endDate: string | null;
  status: string | null;
}

export interface HealthGoalMessage {
  message: string;
}

export interface DoctorHealthGoalPatient {
  patientId: string;
  userId: string;
  fullName: string;
  lastAppointmentAt: string | null;
  appointmentCount: number;
}

export interface DoctorHealthGoalMetricType {
  id: number;
  code: string;
  name: string;
  unit: string;
}
