export interface AccountProfile {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  dateOfBirth: string | null;
  gender: string | null;
  avatarUrl: string | null;
  role: string;
  status: string;
}

export interface UpdateAccountProfileRequest {
  fullName: string;
  phone: string;
  dateOfBirth: string | null;
  gender: string | null;
  avatarUrl: string | null;
}

export interface DoctorSelfProfile {
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

export interface UpdateDoctorSelfProfileRequest {
  specialty: string;
  bio: string | null;
  yearsExperience: number | null;
  clinicId: string | null;
  consultationFee: number | null;
}

export interface ClinicOption {
  id: string;
  name: string;
  address: string | null;
  phone: string | null;
}
