import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  CreateHealthMetricRequest,
  HealthMetric,
  MetricType,
} from '../../models/health-metric.model';
import { HealthMetricService } from '../../services/health-metric.service';

@Component({
  selector: 'app-patient-health-metrics',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, DatePipe, DecimalPipe],
  templateUrl: './patient-health-metrics.html',
  styleUrl: './patient-health-metrics.css',
})
export class PatientHealthMetrics implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly healthMetricService = inject(HealthMetricService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  metricTypes: MetricType[] = [];
  metrics: HealthMetric[] = [];

  isLoading = true;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  metricForm = this.formBuilder.nonNullable.group({
    metricTypeId: [0, [Validators.required, Validators.min(1)]],
    value: [0, [Validators.required, Validators.min(0)]],
    recordedAt: [''],
  });

  get metricTypeId() {
    return this.metricForm.controls.metricTypeId;
  }

  get value() {
    return this.metricForm.controls.value;
  }

  get latestMetric(): HealthMetric | null {
    return this.metrics.length > 0 ? this.metrics[0] : null;
  }

  get selectedMetricType(): MetricType | null {
    const id = Number(this.metricTypeId.value);

    return this.metricTypes.find(type => type.id === id) ?? null;
  }

  ngOnInit(): void {
    this.loadData();
  }

  save(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.metricForm.markAllAsTouched();

    if (this.metricForm.invalid) {
      return;
    }

    const value = this.metricForm.getRawValue();

    const request: CreateHealthMetricRequest = {
      metricTypeId: Number(value.metricTypeId),
      value: Number(value.value),
      recordedAt: value.recordedAt
        ? new Date(value.recordedAt).toISOString()
        : null,
    };

    this.isSaving = true;

    this.healthMetricService.create(request).subscribe({
      next: (createdMetric) => {
        this.metrics = [
          createdMetric,
          ...this.metrics.filter(metric => metric.id !== createdMetric.id),
        ];

        this.successMessage = 'Đã ghi nhận chỉ số sức khỏe thành công.';
        this.isSaving = false;

        this.metricForm.reset({
          metricTypeId: 0,
          value: 0,
          recordedAt: '',
        });

        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        console.error('Lỗi tạo chỉ số sức khỏe:', error);

        this.errorMessage = this.getApiErrorMessage(error);
        this.isSaving = false;

        this.changeDetectorRef.detectChanges();
      },
    });
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  getMetricStatus(metric: HealthMetric): string {
    if (
      metric.normalMin !== null &&
      metric.value < metric.normalMin
    ) {
      return 'low';
    }

    if (
      metric.normalMax !== null &&
      metric.value > metric.normalMax
    ) {
      return 'high';
    }

    if (
      metric.normalMin === null &&
      metric.normalMax === null
    ) {
      return 'unknown';
    }

    return 'normal';
  }

  getMetricStatusText(metric: HealthMetric): string {
    const status = this.getMetricStatus(metric);

    switch (status) {
      case 'low':
        return 'Thấp';

      case 'high':
        return 'Cao';

      case 'normal':
        return 'Bình thường';

      default:
        return 'Chưa xác định';
    }
  }

  getNormalRange(metric: HealthMetric): string {
    if (
      metric.normalMin !== null &&
      metric.normalMax !== null
    ) {
      return `${metric.normalMin} - ${metric.normalMax} ${metric.metricTypeUnit}`;
    }

    if (metric.normalMin !== null) {
      return `Từ ${metric.normalMin} ${metric.metricTypeUnit}`;
    }

    if (metric.normalMax !== null) {
      return `Tối đa ${metric.normalMax} ${metric.metricTypeUnit}`;
    }

    return 'Chưa thiết lập';
  }

  private loadData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.healthMetricService.getMetricTypes().subscribe({
      next: (types) => {
        this.metricTypes = types;

        this.healthMetricService.getMyMetrics().subscribe({
          next: (metrics) => {
            this.metrics = metrics;
            this.isLoading = false;

            this.changeDetectorRef.detectChanges();
          },
          error: (error) => {
            console.error('Lỗi tải lịch sử chỉ số:', error);

            this.errorMessage = this.getApiErrorMessage(error);
            this.isLoading = false;

            this.changeDetectorRef.detectChanges();
          },
        });
      },
      error: (error) => {
        console.error('Lỗi tải loại chỉ số:', error);

        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;

        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') {
      return error.error;
    }

    if (error?.error?.message) {
      return error.error.message;
    }

    if (error?.error?.errors) {
      const validationErrors = Object.values(
        error.error.errors,
      ).flat();

      if (validationErrors.length > 0) {
        return String(validationErrors[0]);
      }
    }

    return 'Không thể xử lý dữ liệu sức khỏe. Vui lòng thử lại.';
  }
}
