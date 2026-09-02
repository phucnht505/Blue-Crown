import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  AdminDoctor,
  AdminDoctorMessage,
  AdminDoctorMeta,
  CreateAdminDoctorRequest,
  UpdateAdminDoctorRequest,
} from '../models/admin-doctor.model';

@Injectable({
  providedIn: 'root',
})
export class AdminDoctorService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/DoctorProfile';

  getAll(search = '', specialty = '', status = ''): Observable<AdminDoctor[]> {
    let params = new HttpParams();

    if (search.trim()) params = params.set('search', search.trim());
    if (specialty) params = params.set('specialty', specialty);
    if (status) params = params.set('status', status);

    return this.http.get<AdminDoctor[]>(`${this.apiUrl}/admin`, { params });
  }

  getMeta(): Observable<AdminDoctorMeta> {
    return this.http.get<AdminDoctorMeta>(`${this.apiUrl}/admin/meta`);
  }

  getById(id: string): Observable<AdminDoctor> {
    return this.http.get<AdminDoctor>(`${this.apiUrl}/admin/${id}`);
  }

  create(request: CreateAdminDoctorRequest): Observable<AdminDoctor> {
    return this.http.post<AdminDoctor>(`${this.apiUrl}/admin`, request);
  }

  update(id: string, request: UpdateAdminDoctorRequest): Observable<AdminDoctor> {
    return this.http.put<AdminDoctor>(`${this.apiUrl}/admin/${id}`, request);
  }

  updateStatus(id: string, status: string): Observable<AdminDoctor> {
    return this.http.patch<AdminDoctor>(`${this.apiUrl}/admin/${id}/status`, { status });
  }

  verify(id: string, verified: boolean): Observable<AdminDoctorMessage> {
    return this.http.put<AdminDoctorMessage>(`${this.apiUrl}/${id}/verify`, verified);
  }

  deactivate(id: string): Observable<AdminDoctorMessage> {
    return this.http.delete<AdminDoctorMessage>(`${this.apiUrl}/admin/${id}`);
  }
}

