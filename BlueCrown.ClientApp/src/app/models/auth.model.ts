export interface RegisterRequest {
  fullName: string;
  email: string;
  phone: string;
  password: string;
  dateOfBirth: string | null;
  gender: string | null;
}

export interface RegisterResponse {
  message: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  userID: string;
  fullName: string;
  email: string;
  role: string;
  token: string;
  expiresAt: string;
}

export interface AuthUser {
  userId: string;
  fullName: string;
  email: string;
  role: string;
  expiresAt: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  otp: string;
  newPassword: string;
  confirmPassword: string;
}

export interface AuthMessageResponse {
  message: string;
}
