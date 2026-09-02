import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  AdminUser,
  AdminUserDetail,
  AdminUserMessageResponse,
  CreateAdminUserRequest,
  UpdateAdminUserRequest,
  UpdateAdminUserStatusRequest,
} from '../models/admin-user.model';

@Injectable({
  providedIn: 'root',
})
export class AdminUserService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/Users';

  getAll(search = '', role = '', status = ''): Observable<AdminUser[]> {
    let params = new HttpParams();

    if (search.trim()) params = params.set('search', search.trim());
    if (role) params = params.set('role', role);
    if (status) params = params.set('status', status);

    return this.http.get<AdminUser[]>(this.apiUrl, { params });
  }

  getById(id: string): Observable<AdminUserDetail> {
    return this.http.get<AdminUserDetail>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateAdminUserRequest): Observable<AdminUserDetail> {
    return this.http.post<AdminUserDetail>(this.apiUrl, request);
  }

  update(id: string, request: UpdateAdminUserRequest): Observable<AdminUserDetail> {
    return this.http.put<AdminUserDetail>(`${this.apiUrl}/${id}`, request);
  }

  updateStatus(id: string, request: UpdateAdminUserStatusRequest): Observable<AdminUserDetail> {
    return this.http.patch<AdminUserDetail>(`${this.apiUrl}/${id}/status`, request);
  }

  delete(id: string): Observable<AdminUserMessageResponse> {
    return this.http.delete<AdminUserMessageResponse>(`${this.apiUrl}/${id}`);
  }
}

