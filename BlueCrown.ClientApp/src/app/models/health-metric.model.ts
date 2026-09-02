export interface MetricType {
  id: number;
  code: string;
  name: string;
  unit: string;
  normalMin: number | null;
  normalMax: number | null;
}

export interface HealthMetric {
  id: string;
  patientId: string;
  metricTypeId: number;
  metricTypeCode: string;
  metricTypeName: string;
  metricTypeUnit: string;
  value: number;
  recordedAt: string;
  normalMin: number | null;
  normalMax: number | null;
}

export interface CreateHealthMetricRequest {
  metricTypeId: number;
  value: number;
  recordedAt: string | null;
}
