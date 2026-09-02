import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminMetricType, Clinic, ClinicRequest, MetricTypeRequest } from '../../models/admin-category.model';
import { AdminCategoryService } from '../../services/admin-category.service';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './admin-categories.html',
  styleUrl: './admin-categories.css',
})
export class AdminCategories implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly categoryService = inject(AdminCategoryService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  activeTab: 'clinics' | 'metrics' = 'clinics';
  clinics: Clinic[] = [];
  metricTypes: AdminMetricType[] = [];
  clinicSearch = '';
  metricSearch = '';
  editingClinicId: string | null = null;
  editingMetricId: number | null = null;
  isClinicFormOpen = false;
  isMetricFormOpen = false;
  isLoadingClinics = false;
  isLoadingMetrics = false;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  clinicForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(255)]],
    address: [''],
    phone: ['', Validators.pattern(/^(0[35789]\d{8}|\+84[35789]\d{8})$/)],
  });

  metricForm = this.formBuilder.nonNullable.group({
    code: ['', Validators.required],
    name: ['', Validators.required],
    unit: ['', Validators.required],
    normalMin: [''],
    normalMax: [''],
  });

  ngOnInit(): void {
    this.loadClinics();
    this.loadMetricTypes();
  }

  get filteredClinics(): Clinic[] {
    const keyword = this.clinicSearch.trim().toLowerCase();

    if (!keyword)
      return this.clinics;

    return this.clinics.filter(x =>
      x.name.toLowerCase().includes(keyword) ||
      (x.address ?? '').toLowerCase().includes(keyword) ||
      (x.phone ?? '').toLowerCase().includes(keyword)
    );
  }

  get filteredMetricTypes(): AdminMetricType[] {
    const keyword = this.metricSearch.trim().toLowerCase();

    if (!keyword)
      return this.metricTypes;

    return this.metricTypes.filter(x =>
      x.code.toLowerCase().includes(keyword) ||
      x.name.toLowerCase().includes(keyword) ||
      x.unit.toLowerCase().includes(keyword)
    );
  }

  setTab(tab: 'clinics' | 'metrics'): void {
    this.activeTab = tab;
    this.clearMessages();
  }

  loadClinics(): void {
    this.isLoadingClinics = true;
    this.errorMessage = '';

    this.categoryService.getClinics().subscribe({
      next: clinics => {
        this.clinics = clinics;
        this.isLoadingClinics = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoadingClinics = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  loadMetricTypes(): void {
    this.isLoadingMetrics = true;
    this.errorMessage = '';

    this.categoryService.getMetricTypes().subscribe({
      next: metricTypes => {
        this.metricTypes = metricTypes;
        this.isLoadingMetrics = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoadingMetrics = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  openCreateClinic(): void {
    this.clearMessages();
    this.editingClinicId = null;
    this.isClinicFormOpen = true;
    this.clinicForm.reset({ name: '', address: '', phone: '' });
  }

  openEditClinic(clinic: Clinic): void {
    this.clearMessages();
    this.editingClinicId = clinic.id;
    this.isClinicFormOpen = true;

    this.clinicForm.reset({
      name: clinic.name,
      address: clinic.address ?? '',
      phone: clinic.phone ?? '',
    });
  }

  closeClinicForm(): void {
    this.isClinicFormOpen = false;
    this.editingClinicId = null;
  }

  saveClinic(): void {
    this.clearMessages();
    this.clinicForm.markAllAsTouched();

    if (this.clinicForm.invalid)
      return;

    const value = this.clinicForm.getRawValue();

    const request: ClinicRequest = {
      name: value.name.trim(),
      address: value.address.trim() || null,
      phone: value.phone.trim() || null,
    };

    this.isSaving = true;

    if (this.editingClinicId) {
      this.categoryService.updateClinic(this.editingClinicId, request).subscribe({
        next: () => this.handleClinicSuccess('Cập nhật phòng khám thành công.'),
        error: error => this.handleSaveError(error),
      });

      return;
    }

    this.categoryService.createClinic(request).subscribe({
      next: () => this.handleClinicSuccess('Thêm phòng khám thành công.'),
      error: error => this.handleSaveError(error),
    });
  }

  deleteClinic(clinic: Clinic): void {
    this.clearMessages();

    if (!window.confirm(`Bạn có chắc muốn xóa phòng khám "${clinic.name}"?`))
      return;

    this.categoryService.deleteClinic(clinic.id).subscribe({
      next: response => {
        this.successMessage = response.message;
        this.loadClinics();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  openCreateMetric(): void {
    this.clearMessages();
    this.editingMetricId = null;
    this.isMetricFormOpen = true;

    this.metricForm.reset({
      code: '',
      name: '',
      unit: '',
      normalMin: '',
      normalMax: '',
    });
  }

  openEditMetric(metric: AdminMetricType): void {
    this.clearMessages();
    this.editingMetricId = metric.id;
    this.isMetricFormOpen = true;

    this.metricForm.reset({
      code: metric.code,
      name: metric.name,
      unit: metric.unit,
      normalMin: metric.normalMin == null ? '' : String(metric.normalMin),
      normalMax: metric.normalMax == null ? '' : String(metric.normalMax),
    });
  }

  closeMetricForm(): void {
    this.isMetricFormOpen = false;
    this.editingMetricId = null;
  }

  saveMetric(): void {
    this.clearMessages();
    this.metricForm.markAllAsTouched();

    if (this.metricForm.invalid)
      return;

    const value = this.metricForm.getRawValue();
    const normalMin = this.toNullableNumber(value.normalMin);
    const normalMax = this.toNullableNumber(value.normalMax);

    if (normalMin != null && normalMax != null && normalMin > normalMax) {
      this.errorMessage = 'Giá trị bình thường tối thiểu không được lớn hơn tối đa.';
      return;
    }

    const request: MetricTypeRequest = {
      code: value.code.trim().toUpperCase(),
      name: value.name.trim(),
      unit: value.unit.trim(),
      normalMin,
      normalMax,
    };

    this.isSaving = true;

    if (this.editingMetricId !== null) {
      this.categoryService.updateMetricType(this.editingMetricId, request).subscribe({
        next: () => this.handleMetricSuccess('Cập nhật loại chỉ số sức khỏe thành công.'),
        error: error => this.handleSaveError(error),
      });

      return;
    }

    this.categoryService.createMetricType(request).subscribe({
      next: () => this.handleMetricSuccess('Thêm loại chỉ số sức khỏe thành công.'),
      error: error => this.handleSaveError(error),
    });
  }

  deleteMetric(metric: AdminMetricType): void {
    this.clearMessages();

    if (!window.confirm(`Bạn có chắc muốn xóa loại chỉ số "${metric.name}"?`))
      return;

    this.categoryService.deleteMetricType(metric.id).subscribe({
      next: response => {
        this.successMessage = response.message;
        this.loadMetricTypes();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  clearClinicSearch(): void {
    this.clinicSearch = '';
  }

  clearMetricSearch(): void {
    this.metricSearch = '';
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private handleClinicSuccess(message: string): void {
    this.isSaving = false;
    this.isClinicFormOpen = false;
    this.editingClinicId = null;
    this.successMessage = message;
    this.loadClinics();
  }

  private handleMetricSuccess(message: string): void {
    this.isSaving = false;
    this.isMetricFormOpen = false;
    this.editingMetricId = null;
    this.successMessage = message;
    this.loadMetricTypes();
  }

  private handleSaveError(error: any): void {
    this.isSaving = false;
    this.errorMessage = this.getApiErrorMessage(error);
    this.changeDetectorRef.detectChanges();
  }

  private toNullableNumber(value: string): number | null {
    if (value.trim() === '')
      return null;

    const number = Number(value);
    return Number.isFinite(number) ? number : null;
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

    return 'Không thể xử lý yêu cầu. Vui lòng thử lại.';
  }
}
