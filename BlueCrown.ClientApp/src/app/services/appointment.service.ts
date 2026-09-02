import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Appointment, AppointmentDoctor, AppointmentMessage, CreateAppointmentRequest } from '../models/appointment.model';

@Injectable({
  providedIn: 'root',
})
export class AppointmentService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/Appointment';

  getMyAppointments(): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.apiUrl}/my`);
  }

  getDoctors(): Observable<AppointmentDoctor[]> {
    return this.http.get<AppointmentDoctor[]>(`${this.apiUrl}/doctors`);
  }

  create(request: CreateAppointmentRequest): Observable<Appointment> {
    return this.http.post<Appointment>(this.apiUrl, request);
  }

  cancel(id: string): Observable<AppointmentMessage> {
    return this.http.delete<AppointmentMessage>(`${this.apiUrl}/${id}`);
  }

  getDoctorAppointments(): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.apiUrl}/doctor/my`);
  }

  updateDoctorStatus(id: string, status: 'confirmed' | 'cancelled' | 'completed'): Observable<Appointment> {
    return this.http.put<Appointment>(`${this.apiUrl}/doctor/${id}/status`, { status });
  }
}

