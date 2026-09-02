import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateMedicationRequest, Medication, UpdateMedicationRequest } from '../models/medication.model';

@Injectable({
  providedIn: 'root',
})
export class MedicationService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/Medication';

  getAll(): Observable<Medication[]> {
    return this.http.get<Medication[]>(this.apiUrl);
  }

  getById(id: string): Observable<Medication> {
    return this.http.get<Medication>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateMedicationRequest): Observable<Medication> {
    return this.http.post<Medication>(this.apiUrl, request);
  }

  update(id: string, request: UpdateMedicationRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }
}

