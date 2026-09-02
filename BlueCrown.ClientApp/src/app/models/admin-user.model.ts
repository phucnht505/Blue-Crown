export interface AdminUser {
  id: string;
  fullName: string | null;
  email: string | null;
  phone: string | null;
  role: string | null;
  status: string | null;
}

export interface AdminUserDetail extends AdminUser {
  dateOfBirth: string | null;
  gender: string | null;
  avatarUrl: string | null;
}

export interface CreateAdminUserRequest {
  fullName: string;
  email: string;
  phone: string;
  password: string;
  dateOfBirth: string | null;
  gender: string | null;
  role: string;
  status: string;
}

export interface UpdateAdminUserRequest {
  fullName: string;
  email: string;
  phone: string;
  dateOfBirth: string | null;
  gender: string | null;
  avatarUrl: string | null;
  role: string;
  status: string;
}

export interface UpdateAdminUserStatusRequest {
  status: string;
}

export interface AdminUserMessageResponse {
  message: string;
}
