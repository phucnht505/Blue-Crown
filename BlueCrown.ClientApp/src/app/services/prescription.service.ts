import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreatePrescriptionRequest, DispensePrescriptionRequest, Prescription } from '../models/prescription.model';

@Injectable({
  providedIn: 'root',
})
export class PrescriptionService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/Prescription';

  getDoctorPrescriptions(): Observable<Prescription[]> {
    return this.http.get<Prescription[]>(`${this.apiUrl}/doctor/my`);
  }

  getPatientPrescriptions(): Observable<Prescription[]> {
    return this.http.get<Prescription[]>(`${this.apiUrl}/patient/my`);
  }

  create(request: CreatePrescriptionRequest): Observable<Prescription> {
    return this.http.post<Prescription>(this.apiUrl, request);
  }

  getPharmacistPrescriptions(): Observable<Prescription[]> {
    return this.http.get<Prescription[]>(`${this.apiUrl}/pharmacist`);
  }

  getPharmacistPrescriptionById(id: string): Observable<Prescription> {
    return this.http.get<Prescription>(`${this.apiUrl}/pharmacist/${id}`);
  }

  updatePharmacistStatus(id: string, status: 'approved' | 'cancelled'): Observable<Prescription> {
    return this.http.put<Prescription>(`${this.apiUrl}/pharmacist/${id}/status`, { status });
  }

  dispense(id: string, request: DispensePrescriptionRequest): Observable<Prescription> {
    return this.http.post<Prescription>(`${this.apiUrl}/pharmacist/${id}/dispense`, request);
  }
}

