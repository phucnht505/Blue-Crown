import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CreateHealthGoalRequest, HealthGoal, UpdateHealthGoalRequest } from '../../models/health-goal.model';
import { HealthMetric, MetricType } from '../../models/health-metric.model';
import { AuthService } from '../../services/auth.service';
import { HealthGoalService } from '../../services/health-goal.service';
import { HealthMetricService } from '../../services/health-metric.service';

@Component({
  selector: 'app-patient-health-goals',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, DecimalPipe],
  templateUrl: './patient-health-goals.html',
  styleUrl: './patient-health-goals.css',
})
export class PatientHealthGoals implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly healthGoalService = inject(HealthGoalService);
  private readonly healthMetricService = inject(HealthMetricService);
  private readonly authService = inject(AuthService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  goals: HealthGoal[] = [];
  metricTypes: MetricType[] = [];
  metrics: HealthMetric[] = [];

  isLoading = true;
  isSaving = false;
  deletingGoalId: string | null = null;
  editingGoalId: string | null = null;
  errorMessage = '';
  successMessage = '';
  dateErrorMessage = '';

  goalForm = this.formBuilder.nonNullable.group({
    metricTypeId: [0, [Validators.required, Validators.min(1)]],
    targetValue: [0, [Validators.required, Validators.min(0.01)]],
    startDate: [''],
    endDate: [''],
    status: ['active'],
  });

  get metricTypeId() {
    return this.goalForm.controls.metricTypeId;
  }

  get targetValue() {
    return this.goalForm.controls.targetValue;
  }

  get selectedMetricType(): MetricType | null {
    const id = Number(this.metricTypeId.value);
    return this.metricTypes.find(type => type.id === id) ?? null;
  }

  ngOnInit(): void {
    this.loadData();
  }

  canPatientManage(goal: HealthGoal): boolean {
    const user = this.authService.getCurrentUser();

    if (!user)
      return false;

    return goal.createdByRole?.toLowerCase() === 'patient' &&
      goal.createdByUserId === user.userId;
  }

  getCreatorText(goal: HealthGoal): string {
    return goal.createdByRole?.toLowerCase() === 'doctor'
      ? 'Do bác sĩ thiết lập'
      : 'Mục tiêu cá nhân';
  }

  save(): void {
    this.clearMessages();
    this.goalForm.markAllAsTouched();

    if (this.goalForm.invalid)
      return;

    const value = this.goalForm.getRawValue();

    if (value.startDate && value.endDate && value.endDate < value.startDate) {
      this.dateErrorMessage = 'Ngày kết thúc không được nhỏ hơn ngày bắt đầu.';
      return;
    }

    this.isSaving = true;

    if (this.editingGoalId) {
      const goal = this.goals.find(item => item.id === this.editingGoalId);

      if (!goal || !this.canPatientManage(goal)) {
        this.errorMessage = 'Bạn không thể chỉnh sửa mục tiêu sức khỏe do bác sĩ thiết lập.';
        this.isSaving = false;
        return;
      }

      const request: UpdateHealthGoalRequest = {
        metricTypeId: Number(value.metricTypeId),
        targetValue: Number(value.targetValue),
        startDate: value.startDate || null,
        endDate: value.endDate || null,
        status: value.status,
      };

      this.healthGoalService.update(this.editingGoalId, request).subscribe({
        next: response => {
          this.successMessage = response.message;
          this.isSaving = false;
          this.editingGoalId = null;
          this.resetForm();
          this.reloadGoals();
        },
        error: error => {
          this.errorMessage = this.getApiErrorMessage(error);
          this.isSaving = false;
          this.changeDetectorRef.detectChanges();
        },
      });

      return;
    }

    const request: CreateHealthGoalRequest = {
      metricTypeId: Number(value.metricTypeId),
      targetValue: Number(value.targetValue),
      startDate: value.startDate || null,
      endDate: value.endDate || null,
    };

    this.healthGoalService.create(request).subscribe({
      next: () => {
        this.successMessage = 'Tạo mục tiêu sức khỏe thành công.';
        this.isSaving = false;
        this.resetForm();
        this.reloadGoals();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  startEdit(goal: HealthGoal): void {
    this.clearMessages();

    if (!this.canPatientManage(goal)) {
      this.errorMessage = 'Bạn không thể chỉnh sửa mục tiêu sức khỏe do bác sĩ thiết lập.';
      return;
    }

    this.editingGoalId = goal.id;

    this.goalForm.patchValue({
      metricTypeId: goal.metricTypeId,
      targetValue: goal.targetValue ?? 0,
      startDate: goal.startDate ?? '',
      endDate: goal.endDate ?? '',
      status: this.normalizeStatus(goal.status),
    });

    window.scrollTo({
      top: 0,
      behavior: 'smooth',
    });
  }

  cancelEdit(): void {
    this.editingGoalId = null;
    this.clearMessages();
    this.resetForm();
  }

  deleteGoal(goal: HealthGoal): void {
    if (!this.canPatientManage(goal)) {
      this.errorMessage = 'Bạn không thể xóa mục tiêu sức khỏe do bác sĩ thiết lập.';
      return;
    }

    const confirmed = window.confirm(
      `Bạn có chắc chắn muốn xóa mục tiêu "${goal.metricTypeName}"?`
    );

    if (!confirmed)
      return;

    this.clearMessages();
    this.deletingGoalId = goal.id;

    this.healthGoalService.delete(goal.id).subscribe({
      next: response => {
        this.goals = this.goals.filter(item => item.id !== goal.id);
        this.successMessage = response.message;
        this.deletingGoalId = null;

        if (this.editingGoalId === goal.id)
          this.cancelEdit();

        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.deletingGoalId = null;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  getLatestMetric(goal: HealthGoal): HealthMetric | null {
    return this.metrics.find(
      metric => metric.metricTypeId === goal.metricTypeId
    ) ?? null;
  }

  getDifferenceText(goal: HealthGoal): string {
    const metric = this.getLatestMetric(goal);

    if (!metric || goal.targetValue === null)
      return 'Chưa có dữ liệu hiện tại';

    const difference = metric.value - goal.targetValue;

    if (difference === 0)
      return 'Đã đạt đúng giá trị mục tiêu';

    const amount = Math.abs(difference).toLocaleString(
      'vi-VN',
      { maximumFractionDigits: 2 }
    );

    if (difference > 0)
      return `Hiện cao hơn mục tiêu ${amount} ${goal.metricTypeUnit}`;

    return `Hiện thấp hơn mục tiêu ${amount} ${goal.metricTypeUnit}`;
  }

  getStatusText(status: string | null): string {
    switch (this.normalizeStatus(status)) {
      case 'completed':
        return 'Hoàn thành';
      case 'cancelled':
        return 'Đã hủy';
      default:
        return 'Đang thực hiện';
    }
  }

  getStatusClass(status: string | null): string {
    return `status-${this.normalizeStatus(status)}`;
  }

  formatDate(value: string | null): string {
    if (!value)
      return 'Không đặt';

    const parts = value.split('-');

    if (parts.length !== 3)
      return value;

    return `${parts[2]}/${parts[1]}/${parts[0]}`;
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.dateErrorMessage = '';
  }

  private loadData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      goals: this.healthGoalService.getMyGoals(),
      metricTypes: this.healthMetricService.getMetricTypes(),
      metrics: this.healthMetricService.getMyMetrics(),
    }).subscribe({
      next: result => {
        this.goals = result.goals;
        this.metricTypes = result.metricTypes;
        this.metrics = result.metrics;
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        console.error('Lỗi tải mục tiêu sức khỏe:', error);
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private reloadGoals(): void {
    this.healthGoalService.getMyGoals().subscribe({
      next: goals => {
        this.goals = goals;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private resetForm(): void {
    this.goalForm.reset({
      metricTypeId: 0,
      targetValue: 0,
      startDate: '',
      endDate: '',
      status: 'active',
    });
  }

  private normalizeStatus(status: string | null): string {
    const value = status?.trim().toLowerCase();

    if (value === 'completed' || value === 'cancelled')
      return value;

    return 'active';
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string')
      return error.error;

    if (error?.error?.message)
      return error.error.message;

    if (error?.error?.errors) {
      const errors = Object.values(error.error.errors).flat();

      if (errors.length > 0)
        return String(errors[0]);
    }

    return 'Không thể xử lý mục tiêu sức khỏe. Vui lòng thử lại.';
  }
}
