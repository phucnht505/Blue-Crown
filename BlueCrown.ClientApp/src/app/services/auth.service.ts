import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import {
  AuthMessageResponse,
  AuthUser,
  ForgotPasswordRequest,
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
  ResetPasswordRequest,
} from '../models/auth.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/Auth';
  private readonly tokenKey = 'blue-crown-token';
  private readonly userKey = 'blue-crown-user';

  private readonly currentUserSubject = new BehaviorSubject<AuthUser | null>(this.loadCurrentUser());

  readonly currentUser$ = this.currentUserSubject.asObservable();

  register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request);
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<AuthMessageResponse> {
    return this.http.post<AuthMessageResponse>(`${this.apiUrl}/forgot-password`, request);
  }

  resetPassword(request: ResetPasswordRequest): Observable<AuthMessageResponse> {
    return this.http.post<AuthMessageResponse>(`${this.apiUrl}/reset-password`, request);
  }

  saveSession(response: LoginResponse): void {
    const user: AuthUser = {
      userId: response.userID,
      fullName: response.fullName,
      email: response.email,
      role: response.role,
      expiresAt: response.expiresAt,
    };

    localStorage.setItem(this.tokenKey, response.token);
    localStorage.setItem(this.userKey, JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  updateCurrentUserProfile(fullName: string): void {
    const currentUser = this.currentUserSubject.value;

    if (!currentUser)
      return;

    const updatedUser: AuthUser = {
      ...currentUser,
      fullName,
    };

    localStorage.setItem(this.userKey, JSON.stringify(updatedUser));
    this.currentUserSubject.next(updatedUser);
  }

  getToken(): string | null {
    if (!this.isAuthenticated())
      return null;

    return localStorage.getItem(this.tokenKey);
  }

  getCurrentUser(): AuthUser | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem(this.tokenKey);
    const user = this.loadStoredUser();

    if (!token || !user)
      return false;

    if (new Date(user.expiresAt).getTime() <= Date.now()) {
      this.logout();
      return false;
    }

    return true;
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.currentUserSubject.next(null);
  }

  private loadCurrentUser(): AuthUser | null {
    const token = localStorage.getItem(this.tokenKey);
    const user = this.loadStoredUser();

    if (!token || !user)
      return null;

    if (new Date(user.expiresAt).getTime() <= Date.now()) {
      localStorage.removeItem(this.tokenKey);
      localStorage.removeItem(this.userKey);
      return null;
    }

    return user;
  }

  private loadStoredUser(): AuthUser | null {
    const data = localStorage.getItem(this.userKey);

    if (!data)
      return null;

    try {
      return JSON.parse(data) as AuthUser;
    } catch {
      return null;
    }
  }
}

