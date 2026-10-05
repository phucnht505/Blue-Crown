import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AdminUser, AdminUserDetail } from '../models/admin-user.model';
import {
  AdminPharmacistCreateRequest,
  AdminPharmacistUpdateRequest,
  UpdateAdminPharmacistStatusRequest,
} from '../models/admin-pharmacist.model';

@Injectable({
  providedIn: 'root',
})
export class AdminPharmacistService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/AdminPharmacist';

  getAll(search = '', status = ''): Observable<AdminUser[]> {
    let params = new HttpParams();

    if (search.trim())
      params = params.set('search', search.trim());

    if (status)
      params = params.set('status', status);

    return this.http.get<AdminUser[]>(this.apiUrl, { params });
  }

  getById(id: string): Observable<AdminUserDetail> {
    return this.http.get<AdminUserDetail>(`${this.apiUrl}/${id}`);
  }

  create(request: AdminPharmacistCreateRequest): Observable<AdminUserDetail> {
    return this.http.post<AdminUserDetail>(this.apiUrl, request);
  }

  update(
    id: string,
    request: AdminPharmacistUpdateRequest
  ): Observable<AdminUserDetail> {
    return this.http.put<AdminUserDetail>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  updateStatus(
    id: string,
    request: UpdateAdminPharmacistStatusRequest
  ): Observable<AdminUserDetail> {
    return this.http.patch<AdminUserDetail>(
      `${this.apiUrl}/${id}/status`,
      request
    );
  }

  delete(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}/${id}/hard`
    );
  }
}
