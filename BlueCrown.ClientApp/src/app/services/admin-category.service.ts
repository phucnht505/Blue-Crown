import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AdminMetricType, Clinic, ClinicRequest, MetricTypeRequest } from '../models/admin-category.model';

@Injectable({
  providedIn: 'root',
})
export class AdminCategoryService {
  private readonly http = inject(HttpClient);
  private readonly clinicUrl = '/api/Clinic';
  private readonly metricTypeUrl = '/api/MetricType';

  getClinics(): Observable<Clinic[]> {
    return this.http.get<Clinic[]>(this.clinicUrl);
  }

  getClinicById(id: string): Observable<Clinic> {
    return this.http.get<Clinic>(`${this.clinicUrl}/${id}`);
  }

  createClinic(request: ClinicRequest): Observable<Clinic> {
    return this.http.post<Clinic>(this.clinicUrl, request);
  }

  updateClinic(id: string, request: ClinicRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.clinicUrl}/${id}`, request);
  }

  deleteClinic(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.clinicUrl}/${id}`);
  }

  getMetricTypes(): Observable<AdminMetricType[]> {
    return this.http.get<AdminMetricType[]>(this.metricTypeUrl);
  }

  getMetricTypeById(id: number): Observable<AdminMetricType> {
    return this.http.get<AdminMetricType>(`${this.metricTypeUrl}/${id}`);
  }

  createMetricType(request: MetricTypeRequest): Observable<AdminMetricType> {
    return this.http.post<AdminMetricType>(this.metricTypeUrl, request);
  }

  updateMetricType(id: number, request: MetricTypeRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.metricTypeUrl}/${id}`, request);
  }

  deleteMetricType(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.metricTypeUrl}/${id}`);
  }
}

