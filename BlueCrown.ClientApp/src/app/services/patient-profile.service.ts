import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { PatientProfile, PatientProfileMessage, PatientProfileRequest } from '../models/patient-profile.model';

@Injectable({
  providedIn: 'root',
})
export class PatientProfileService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/PatientProfile';

  getMyProfile(): Observable<PatientProfile> {
    return this.http.get<PatientProfile>(`${this.apiUrl}/me`);
  }

  create(request: PatientProfileRequest): Observable<PatientProfileMessage> {
    return this.http.post<PatientProfileMessage>(this.apiUrl, request);
  }

  update(request: PatientProfileRequest): Observable<PatientProfileMessage> {
    return this.http.put<PatientProfileMessage>(this.apiUrl, request);
  }
}

