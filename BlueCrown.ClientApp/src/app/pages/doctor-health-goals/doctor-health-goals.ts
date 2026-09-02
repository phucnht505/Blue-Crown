import { AuthService } from '../../services/auth.service';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import {
  CreateHealthGoalRequest,
  DoctorHealthGoalMetricType,
  DoctorHealthGoalPatient,
  HealthGoal,
  UpdateHealthGoalRequest,
} from '../../models/health-goal.model';
import { HealthGoalService } from '../../services/health-goal.service';

@Component({
  selector: 'app-doctor-health-goals',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, DatePipe, DecimalPipe],
  templateUrl: './doctor-health-goals.html',
  styleUrl: './doctor-health-goals.css',
})
export class DoctorHealthGoals implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly healthGoalService = inject(HealthGoalService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);
  private readonly authService = inject(AuthService);

  patients: DoctorHealthGoalPatient[] = [];
  metricTypes: DoctorHealthGoalMetricType[] = [];
  goals: HealthGoal[] = [];

  selectedPatientId = '';
  editingGoalId: string | null = null;
  cancellingGoalId: string | null = null;

  isLoading = true;
  isLoadingGoals = false;
  isSaving = false;

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

  get selectedPatient(): DoctorHealthGoalPatient | null {
    return this.patients.find(x => x.patientId === this.selectedPatientId) ?? null;
  }

  get selectedMetricType(): DoctorHealthGoalMetricType | null {
    const id = Number(this.goalForm.controls.metricTypeId.value);
    return this.metricTypes.find(x => x.id === id) ?? null;
  }

  ngOnInit(): void {
    this.loadInitialData();
  }

  selectPatient(patientId: string): void {
    this.selectedPatientId = patientId;
    this.editingGoalId = null;
    this.goals = [];
    this.clearMessages();
    this.resetForm();

    if (!patientId)
      return;

    this.loadGoals();
  }

  save(): void {
    this.clearMessages();
    this.goalForm.markAllAsTouched();

    if (!this.selectedPatientId) {
      this.errorMessage = 'Vui lòng chọn bệnh nhân.';
      return;
    }

    if (this.goalForm.invalid)
      return;

    const value = this.goalForm.getRawValue();

    if (value.startDate && value.endDate && value.endDate < value.startDate) {
      this.dateErrorMessage = 'Ngày kết thúc không được nhỏ hơn ngày bắt đầu.';
      return;
    }

    this.isSaving = true;

    if (this.editingGoalId) {
      const request: UpdateHealthGoalRequest = {
        metricTypeId: Number(value.metricTypeId),
        targetValue: Number(value.targetValue),
        startDate: value.startDate || null,
        endDate: value.endDate || null,
        status: value.status,
      };

      this.healthGoalService.updateForPatient(
        this.selectedPatientId,
        this.editingGoalId,
        request
      ).subscribe({
        next: response => {
          this.successMessage = response.message;
          this.isSaving = false;
          this.editingGoalId = null;
          this.resetForm();
          this.loadGoals(false);
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

    this.healthGoalService.createForPatient(this.selectedPatientId, request).subscribe({
      next: () => {
        this.successMessage = 'Tạo mục tiêu sức khỏe cho bệnh nhân thành công.';
        this.isSaving = false;
        this.resetForm();
        this.loadGoals(false);
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  startEdit(goal: HealthGoal): void {
    if (!this.canDoctorManage(goal)) {
      this.errorMessage = 'Bạn chỉ có thể chỉnh sửa mục tiêu sức khỏe do chính mình thiết lập.';
      return;
    }
    this.clearMessages();
    this.editingGoalId = goal.id;

    this.goalForm.patchValue({
      metricTypeId: goal.metricTypeId,
      targetValue: goal.targetValue ?? 0,
      startDate: goal.startDate ?? '',
      endDate: goal.endDate ?? '',
      status: this.normalizeStatus(goal.status),
    });

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit(): void {
    this.editingGoalId = null;
    this.clearMessages();
    this.resetForm();
  }

  cancelGoal(goal: HealthGoal): void {
    if (!this.canDoctorManage(goal)) {
      this.errorMessage = 'Bạn chỉ có thể hủy mục tiêu sức khỏe do chính mình thiết lập.';
      return;
    }
    if (!this.selectedPatientId)
      return;

    if (goal.status?.toLowerCase() === 'cancelled')
      return;

    if (!window.confirm(`Hủy mục tiêu "${goal.metricTypeName}" của bệnh nhân ${this.selectedPatient?.fullName}?`))
      return;

    this.clearMessages();
    this.cancellingGoalId = goal.id;

    this.healthGoalService.cancelForPatient(this.selectedPatientId, goal.id).subscribe({
      next: response => {
        this.successMessage = response.message;
        this.cancellingGoalId = null;
        this.loadGoals(false);
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.cancellingGoalId = null;
        this.changeDetectorRef.detectChanges();
      },
    });
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

  private loadInitialData(): void {
    this.isLoading = true;

    forkJoin({
      patients: this.healthGoalService.getDoctorPatients(),
      metricTypes: this.healthGoalService.getDoctorMetricTypes(),
    }).subscribe({
      next: result => {
        this.patients = result.patients;
        this.metricTypes = result.metricTypes;

        if (this.patients.length > 0) {
          this.selectedPatientId = this.patients[0].patientId;
          this.loadGoals();
        }

        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private loadGoals(showLoading = true): void {
    if (!this.selectedPatientId)
      return;

    if (showLoading)
      this.isLoadingGoals = true;

    this.healthGoalService.getDoctorPatientGoals(this.selectedPatientId).subscribe({
      next: goals => {
        this.goals = goals;
        this.isLoadingGoals = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoadingGoals = false;
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
  canDoctorManage(goal: HealthGoal): boolean {
    const user = this.authService.getCurrentUser();

    if (!user)
      return false;

    return goal.createdByRole?.toLowerCase() === 'doctor' &&
      goal.createdByUserId === user.userId;
  }

  getCreatorText(goal: HealthGoal): string {
    if (goal.createdByRole?.toLowerCase() === 'patient')
      return 'Do Patient tự tạo';

    if (this.canDoctorManage(goal))
      return 'Do bạn thiết lập';

    return 'Do bác sĩ khác thiết lập';
  }
}
