export interface AdminPharmacistCreateRequest {
  fullName: string;
  email: string;
  phone: string;
  password: string;
  dateOfBirth: string | null;
  gender: string | null;
  status: string;
}

export interface AdminPharmacistUpdateRequest {
  fullName: string;
  email: string;
  phone: string;
  dateOfBirth: string | null;
  gender: string | null;
  avatarUrl: string | null;
  status: string;
}

export interface UpdateAdminPharmacistStatusRequest {
  status: string;
}
