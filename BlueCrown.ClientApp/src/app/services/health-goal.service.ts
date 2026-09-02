import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateHealthGoalRequest,
  DoctorHealthGoalMetricType,
  DoctorHealthGoalPatient,
  HealthGoal,
  HealthGoalMessage,
  UpdateHealthGoalRequest,
} from '../models/health-goal.model';

@Injectable({
  providedIn: 'root',
})
export class HealthGoalService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/HealthGoal';

  getMyGoals(): Observable<HealthGoal[]> {
    return this.http.get<HealthGoal[]>(`${this.apiUrl}/my`);
  }

  getById(id: string): Observable<HealthGoal> {
    return this.http.get<HealthGoal>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateHealthGoalRequest): Observable<HealthGoal> {
    return this.http.post<HealthGoal>(this.apiUrl, request);
  }

  update(id: string, request: UpdateHealthGoalRequest): Observable<HealthGoalMessage> {
    return this.http.put<HealthGoalMessage>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<HealthGoalMessage> {
    return this.http.delete<HealthGoalMessage>(`${this.apiUrl}/${id}`);
  }

  getDoctorPatients(): Observable<DoctorHealthGoalPatient[]> {
    return this.http.get<DoctorHealthGoalPatient[]>(`${this.apiUrl}/doctor/patients`);
  }

  getDoctorMetricTypes(): Observable<DoctorHealthGoalMetricType[]> {
    return this.http.get<DoctorHealthGoalMetricType[]>(`${this.apiUrl}/doctor/metric-types`);
  }

  getDoctorPatientGoals(patientId: string): Observable<HealthGoal[]> {
    return this.http.get<HealthGoal[]>(`${this.apiUrl}/doctor/patient/${patientId}`);
  }

  createForPatient(patientId: string, request: CreateHealthGoalRequest): Observable<HealthGoal> {
    return this.http.post<HealthGoal>(`${this.apiUrl}/doctor/patient/${patientId}`, request);
  }

  updateForPatient(patientId: string, id: string, request: UpdateHealthGoalRequest): Observable<HealthGoalMessage> {
    return this.http.put<HealthGoalMessage>(`${this.apiUrl}/doctor/patient/${patientId}/${id}`, request);
  }

  cancelForPatient(patientId: string, id: string): Observable<HealthGoalMessage> {
    return this.http.put<HealthGoalMessage>(`${this.apiUrl}/doctor/patient/${patientId}/${id}/cancel`, {});
  }
}

