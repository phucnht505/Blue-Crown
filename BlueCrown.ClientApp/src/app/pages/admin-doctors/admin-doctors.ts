import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminDoctor, AdminDoctorMeta, CreateAdminDoctorRequest, UpdateAdminDoctorRequest } from '../../models/admin-doctor.model';
import { AdminDoctorService } from '../../services/admin-doctor.service';

@Component({
  selector: 'app-admin-doctors',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './admin-doctors.html',
  styleUrl: './admin-doctors.css',
})
export class AdminDoctors implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly doctorService = inject(AdminDoctorService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  doctors: AdminDoctor[] = [];
  meta: AdminDoctorMeta = { specialties: [], clinics: [] };
  selectedDoctor: AdminDoctor | null = null;
  editingId: string | null = null;
  isLoading = false;
  isSaving = false;
  isFormOpen = false;
  errorMessage = '';
  successMessage = '';

  filterForm = this.formBuilder.nonNullable.group({
    search: [''],
    specialty: [''],
    status: [''],
  });

  doctorForm = this.formBuilder.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50), Validators.pattern(/^[\p{L}\s]+$/u)]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required, Validators.pattern(/^(03|05|07|08|09)\d{8}$/)]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/)]],
    dateOfBirth: [''],
    gender: [''],
    avatarUrl: [''],
    specialty: ['', [Validators.required, Validators.maxLength(100)]],
    licenseNumber: ['', [Validators.required, Validators.maxLength(100)]],
    licenseVerified: [false],
    bio: ['', Validators.maxLength(2000)],
    yearsExperience: ['', [Validators.min(0), Validators.max(80)]],
    clinicId: [''],
    consultationFee: ['', Validators.min(0)],
    status: ['active', Validators.required],
  });

  ngOnInit(): void {
    this.loadMeta();
    this.loadDoctors();
  }

  loadMeta(): void {
    this.doctorService.getMeta().subscribe({
      next: meta => {
        this.meta = meta;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  loadDoctors(): void {
    const filter = this.filterForm.getRawValue();
    this.isLoading = true;
    this.errorMessage = '';

    this.doctorService.getAll(filter.search, filter.specialty, filter.status).subscribe({
      next: doctors => {
        this.doctors = doctors;
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

  resetFilters(): void {
    this.filterForm.reset({ search: '', specialty: '', status: '' });
    this.loadDoctors();
  }

  openCreate(): void {
    this.clearMessages();
    this.editingId = null;
    this.selectedDoctor = null;
    this.isFormOpen = true;

    this.doctorForm.controls.password.setValidators([
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/),
    ]);
    this.doctorForm.controls.password.updateValueAndValidity();

    this.doctorForm.reset({
      fullName: '',
      email: '',
      phone: '',
      password: '',
      dateOfBirth: '',
      gender: '',
      avatarUrl: '',
      specialty: '',
      licenseNumber: '',
      licenseVerified: false,
      bio: '',
      yearsExperience: '',
      clinicId: '',
      consultationFee: '',
      status: 'active',
    });
  }

  openEdit(id: string): void {
    this.clearMessages();

    this.doctorService.getById(id).subscribe({
      next: doctor => {
        this.editingId = id;
        this.selectedDoctor = doctor;
        this.isFormOpen = true;

        this.doctorForm.controls.password.clearValidators();
        this.doctorForm.controls.password.updateValueAndValidity();

        this.doctorForm.reset({
          fullName: doctor.fullName ?? '',
          email: doctor.email ?? '',
          phone: doctor.phone ?? '',
          password: '',
          dateOfBirth: doctor.dateOfBirth ?? '',
          gender: doctor.gender ?? '',
          avatarUrl: doctor.avatarUrl ?? '',
          specialty: doctor.specialty,
          licenseNumber: doctor.licenseNumber,
          licenseVerified: doctor.licenseVerified === true,
          bio: doctor.bio ?? '',
          yearsExperience: doctor.yearsExperience == null ? '' : String(doctor.yearsExperience),
          clinicId: doctor.clinicId ?? '',
          consultationFee: doctor.consultationFee == null ? '' : String(doctor.consultationFee),
          status: doctor.userStatus ?? 'active',
        });

        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  viewDetail(id: string): void {
    this.clearMessages();

    this.doctorService.getById(id).subscribe({
      next: doctor => {
        this.selectedDoctor = doctor;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  closeForm(): void {
    this.isFormOpen = false;
    this.editingId = null;
  }

  closeDetail(): void {
    this.selectedDoctor = null;
  }

  submit(): void {
    this.clearMessages();
    this.doctorForm.markAllAsTouched();

    if (this.doctorForm.invalid)
      return;

    const value = this.doctorForm.getRawValue();
    this.isSaving = true;

    if (!this.editingId) {
      const request: CreateAdminDoctorRequest = {
        fullName: value.fullName.trim(),
        email: value.email.trim().toLowerCase(),
        phone: value.phone.trim(),
        password: value.password,
        dateOfBirth: value.dateOfBirth || null,
        gender: value.gender || null,
        avatarUrl: value.avatarUrl.trim() || null,
        specialty: value.specialty.trim(),
        licenseNumber: value.licenseNumber.trim(),
        licenseVerified: value.licenseVerified,
        bio: value.bio.trim() || null,
        yearsExperience: this.toNullableNumber(value.yearsExperience),
        clinicId: value.clinicId || null,
        consultationFee: this.toNullableNumber(value.consultationFee),
        status: value.status,
      };

      this.doctorService.create(request).subscribe({
        next: () => this.handleSaveSuccess('Thêm bác sĩ thành công.'),
        error: error => this.handleSaveError(error),
      });

      return;
    }

    const request: UpdateAdminDoctorRequest = {
      fullName: value.fullName.trim(),
      email: value.email.trim().toLowerCase(),
      phone: value.phone.trim(),
      dateOfBirth: value.dateOfBirth || null,
      gender: value.gender || null,
      avatarUrl: value.avatarUrl.trim() || null,
      specialty: value.specialty.trim(),
      licenseNumber: value.licenseNumber.trim(),
      licenseVerified: value.licenseVerified,
      bio: value.bio.trim() || null,
      yearsExperience: this.toNullableNumber(value.yearsExperience),
      clinicId: value.clinicId || null,
      consultationFee: this.toNullableNumber(value.consultationFee),
      status: value.status,
    };

    this.doctorService.update(this.editingId, request).subscribe({
      next: () => this.handleSaveSuccess('Cập nhật bác sĩ thành công.'),
      error: error => this.handleSaveError(error),
    });
  }

  toggleStatus(doctor: AdminDoctor): void {
    const status = doctor.userStatus === 'active' ? 'suspended' : 'active';
    const text = status === 'suspended' ? 'khóa' : 'mở khóa';

    if (!window.confirm(`Bạn có chắc muốn ${text} tài khoản bác sĩ này?`))
      return;

    this.doctorService.updateStatus(doctor.id, status).subscribe({
      next: () => {
        this.successMessage = status === 'active'
          ? 'Mở khóa bác sĩ thành công.'
          : 'Khóa bác sĩ thành công.';

        this.loadDoctors();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  toggleVerified(doctor: AdminDoctor): void {
    const verified = doctor.licenseVerified !== true;

    this.doctorService.verify(doctor.id, verified).subscribe({
      next: response => {
        this.successMessage = response.message;
        this.loadDoctors();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  deactivate(doctor: AdminDoctor): void {
    if (!window.confirm(`Vô hiệu hóa bác sĩ "${doctor.fullName}"? Hồ sơ và dữ liệu y tế sẽ được giữ lại.`))
      return;

    this.doctorService.deactivate(doctor.id).subscribe({
      next: response => {
        this.successMessage = response.message;
        this.loadDoctors();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  formatMoney(value: number | null): string {
    if (value == null)
      return '-';

    return new Intl.NumberFormat('vi-VN').format(value) + ' đ';
  }

  getGenderLabel(value: string | null): string {
    if (value === 'male') return 'Nam';
    if (value === 'female') return 'Nữ';
    if (value === 'other') return 'Khác';
    return '-';
  }

  getStatusLabel(status: string | null): string {
    if (status === 'active') return 'Hoạt động';
    if (status === 'suspended') return 'Đã khóa';
    if (status === 'pending') return 'Chờ kích hoạt';
    return '-';
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private toNullableNumber(value: string): number | null {
    return value === '' ? null : Number(value);
  }

  private handleSaveSuccess(message: string): void {
    this.isSaving = false;
    this.isFormOpen = false;
    this.editingId = null;
    this.successMessage = message;
    this.loadMeta();
    this.loadDoctors();
  }

  private handleSaveError(error: any): void {
    this.errorMessage = this.getApiErrorMessage(error);
    this.isSaving = false;
    this.changeDetectorRef.detectChanges();
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
