import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateHealthMetricRequest,
  HealthMetric,
  MetricType,
} from '../models/health-metric.model';

@Injectable({
  providedIn: 'root',
})
export class HealthMetricService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/HealthMetric';

  getMetricTypes(): Observable<MetricType[]> {
    return this.http.get<MetricType[]>(`${this.apiUrl}/types`);
  }

  getMyMetrics(): Observable<HealthMetric[]> {
    return this.http.get<HealthMetric[]>(`${this.apiUrl}/my`);
  }

  getLatest(): Observable<HealthMetric> {
    return this.http.get<HealthMetric>(`${this.apiUrl}/latest`);
  }

  create(request: CreateHealthMetricRequest): Observable<HealthMetric> {
    return this.http.post<HealthMetric>(this.apiUrl, request);
  }
}

