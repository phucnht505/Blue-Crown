import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateMedicalRecordRequest, MedicalRecord, UpdateMedicalRecordRequest } from '../models/medical-record.model';

@Injectable({
  providedIn: 'root',
})
export class MedicalRecordService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/MedicalRecord';

  getPatientRecords(): Observable<MedicalRecord[]> {
    return this.http.get<MedicalRecord[]>(`${this.apiUrl}/patient/my`);
  }

  getDoctorRecords(): Observable<MedicalRecord[]> {
    return this.http.get<MedicalRecord[]>(`${this.apiUrl}/doctor/my`);
  }

  getDoctorRecordById(id: string): Observable<MedicalRecord> {
    return this.http.get<MedicalRecord>(`${this.apiUrl}/doctor/${id}`);
  }

  getDoctorRecordByAppointment(appointmentId: string): Observable<MedicalRecord> {
    return this.http.get<MedicalRecord>(`${this.apiUrl}/doctor/appointment/${appointmentId}`);
  }

  create(request: CreateMedicalRecordRequest): Observable<MedicalRecord> {
    return this.http.post<MedicalRecord>(this.apiUrl, request);
  }

  update(id: string, request: UpdateMedicalRecordRequest): Observable<MedicalRecord> {
    return this.http.put<MedicalRecord>(`${this.apiUrl}/${id}`, request);
  }
}

