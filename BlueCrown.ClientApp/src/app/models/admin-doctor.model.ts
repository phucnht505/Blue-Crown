export interface AdminDoctor {
  id: string;
  userId: string;
  fullName: string | null;
  email: string | null;
  phone: string | null;
  dateOfBirth: string | null;
  gender: string | null;
  avatarUrl: string | null;
  userStatus: string | null;
  specialty: string;
  licenseNumber: string;
  licenseVerified: boolean | null;
  bio: string | null;
  yearsExperience: number | null;
  clinicId: string | null;
  clinicName: string | null;
  clinicAddress: string | null;
  clinicPhone: string | null;
  consultationFee: number | null;
  ratingAvg: number | null;
  ratingCount: number | null;
}

export interface DoctorClinicOption {
  id: string;
  name: string;
  address: string | null;
  phone: string | null;
}

export interface AdminDoctorMeta {
  specialties: string[];
  clinics: DoctorClinicOption[];
}

export interface CreateAdminDoctorRequest {
  fullName: string;
  email: string;
  phone: string;
  password: string;
  dateOfBirth: string | null;
  gender: string | null;
  avatarUrl: string | null;
  specialty: string;
  licenseNumber: string;
  licenseVerified: boolean;
  bio: string | null;
  yearsExperience: number | null;
  clinicId: string | null;
  consultationFee: number | null;
  status: string;
}

export interface UpdateAdminDoctorRequest {
  fullName: string;
  email: string;
  phone: string;
  dateOfBirth: string | null;
  gender: string | null;
  avatarUrl: string | null;
  specialty: string;
  licenseNumber: string;
  licenseVerified: boolean;
  bio: string | null;
  yearsExperience: number | null;
  clinicId: string | null;
  consultationFee: number | null;
  status: string;
}

export interface AdminDoctorMessage {
  message: string;
}
