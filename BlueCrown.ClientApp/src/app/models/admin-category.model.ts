export interface Clinic {
  id: string;
  name: string;
  address: string | null;
  phone: string | null;
}

export interface ClinicRequest {
  name: string;
  address: string | null;
  phone: string | null;
}

export interface AdminMetricType {
  id: number;
  code: string;
  name: string;
  unit: string;
  normalMin: number | null;
  normalMax: number | null;
}

export interface MetricTypeRequest {
  code: string;
  name: string;
  unit: string;
  normalMin: number | null;
  normalMax: number | null;
}
