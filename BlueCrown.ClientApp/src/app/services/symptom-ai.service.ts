import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SymptomAnalysisResponse } from '../models/symptom-analysis.model';

@Injectable({
  providedIn: 'root'
})
export class SymptomAiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/SymptomLog';

  analyze(symptomsDescription: string): Observable<SymptomAnalysisResponse> {
    return this.http.post<SymptomAnalysisResponse>(`${this.apiUrl}/analyze`, { symptomsDescription });
  }
}

