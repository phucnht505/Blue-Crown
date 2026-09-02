import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  AccountProfile,
  ClinicOption,
  DoctorSelfProfile,
  UpdateAccountProfileRequest,
  UpdateDoctorSelfProfileRequest,
} from '../models/account-profile.model';

@Injectable({
  providedIn: 'root',
})
export class AccountProfileService {
  private readonly http = inject(HttpClient);
  private readonly accountUrl = '/api/Account';
  private readonly doctorUrl = '/api/DoctorProfile';
  private readonly clinicUrl = '/api/Clinic';

  getMyProfile(): Observable<AccountProfile> {
    return this.http.get<AccountProfile>(`${this.accountUrl}/me`);
  }

  updateMyProfile(request: UpdateAccountProfileRequest): Observable<AccountProfile> {
    return this.http.put<AccountProfile>(`${this.accountUrl}/me`, request);
  }

  getMyDoctorProfile(): Observable<DoctorSelfProfile> {
    return this.http.get<DoctorSelfProfile>(`${this.doctorUrl}/me`);
  }

  updateDoctorProfile(id: string, request: UpdateDoctorSelfProfileRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.doctorUrl}/${id}`, request);
  }

  getClinics(): Observable<ClinicOption[]> {
    return this.http.get<ClinicOption[]>(this.clinicUrl);
  }
}

