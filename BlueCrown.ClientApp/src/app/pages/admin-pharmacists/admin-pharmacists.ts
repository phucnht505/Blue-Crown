import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminUser, AdminUserDetail, CreateAdminUserRequest, UpdateAdminUserRequest } from '../../models/admin-user.model';
import { AdminUserService } from '../../services/admin-user.service';

@Component({
  selector: 'app-admin-pharmacists',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './admin-pharmacists.html',
  styleUrl: './admin-pharmacists.css',
})
export class AdminPharmacists implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly userService = inject(AdminUserService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  pharmacists: AdminUser[] = [];
  selectedPharmacist: AdminUserDetail | null = null;
  editingUserId: string | null = null;
  isLoading = false;
  isSaving = false;
  isFormOpen = false;
  errorMessage = '';
  successMessage = '';

  filterForm = this.formBuilder.nonNullable.group({
    search: [''],
    status: [''],
  });

  pharmacistForm = this.formBuilder.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50), Validators.pattern(/^[\p{L}\s]+$/u)]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required, Validators.pattern(/^(03|05|07|08|09)\d{8}$/)]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/)]],
    dateOfBirth: [''],
    gender: [''],
    avatarUrl: [''],
    status: ['active', Validators.required],
  });

  ngOnInit(): void {
    this.loadPharmacists();
  }

  loadPharmacists(): void {
    const filter = this.filterForm.getRawValue();

    this.isLoading = true;
    this.errorMessage = '';

    this.userService.getAll(filter.search, 'pharmacist', filter.status).subscribe({
      next: users => {
        this.pharmacists = users;
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
    this.filterForm.reset({
      search: '',
      status: '',
    });

    this.loadPharmacists();
  }

  openCreate(): void {
    this.clearMessages();
    this.editingUserId = null;
    this.selectedPharmacist = null;
    this.isFormOpen = true;

    this.pharmacistForm.controls.password.setValidators([
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/),
    ]);
    this.pharmacistForm.controls.password.updateValueAndValidity();

    this.pharmacistForm.reset({
      fullName: '',
      email: '',
      phone: '',
      password: '',
      dateOfBirth: '',
      gender: '',
      avatarUrl: '',
      status: 'active',
    });
  }

  openEdit(id: string): void {
    this.clearMessages();

    this.userService.getById(id).subscribe({
      next: user => {
        if (user.role !== 'pharmacist') {
          this.errorMessage = 'Tài khoản này không phải dược sĩ.';
          this.changeDetectorRef.detectChanges();
          return;
        }

        this.editingUserId = id;
        this.selectedPharmacist = user;
        this.isFormOpen = true;

        this.pharmacistForm.controls.password.clearValidators();
        this.pharmacistForm.controls.password.updateValueAndValidity();

        this.pharmacistForm.reset({
          fullName: user.fullName ?? '',
          email: user.email ?? '',
          phone: user.phone ?? '',
          password: '',
          dateOfBirth: user.dateOfBirth ?? '',
          gender: user.gender ?? '',
          avatarUrl: user.avatarUrl ?? '',
          status: user.status ?? 'active',
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

    this.userService.getById(id).subscribe({
      next: user => {
        if (user.role !== 'pharmacist') {
          this.errorMessage = 'Tài khoản này không phải dược sĩ.';
          this.changeDetectorRef.detectChanges();
          return;
        }

        this.selectedPharmacist = user;
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
    this.editingUserId = null;
  }

  closeDetail(): void {
    this.selectedPharmacist = null;
  }

  submit(): void {
    this.clearMessages();
    this.pharmacistForm.markAllAsTouched();

    if (this.pharmacistForm.invalid)
      return;

    const value = this.pharmacistForm.getRawValue();
    this.isSaving = true;

    if (!this.editingUserId) {
      const request: CreateAdminUserRequest = {
        fullName: value.fullName.trim(),
        email: value.email.trim().toLowerCase(),
        phone: value.phone.trim(),
        password: value.password,
        dateOfBirth: value.dateOfBirth || null,
        gender: value.gender || null,
        role: 'pharmacist',
        status: value.status,
      };

      this.userService.create(request).subscribe({
        next: () => this.handleSaveSuccess('Thêm dược sĩ thành công.'),
        error: error => this.handleSaveError(error),
      });

      return;
    }

    const request: UpdateAdminUserRequest = {
      fullName: value.fullName.trim(),
      email: value.email.trim().toLowerCase(),
      phone: value.phone.trim(),
      dateOfBirth: value.dateOfBirth || null,
      gender: value.gender || null,
      avatarUrl: value.avatarUrl.trim() || null,
      role: 'pharmacist',
      status: value.status,
    };

    this.userService.update(this.editingUserId, request).subscribe({
      next: () => this.handleSaveSuccess('Cập nhật dược sĩ thành công.'),
      error: error => this.handleSaveError(error),
    });
  }

  toggleStatus(pharmacist: AdminUser): void {
    const newStatus = pharmacist.status === 'active' ? 'suspended' : 'active';

    const message = newStatus === 'suspended'
      ? `Bạn có chắc muốn khóa tài khoản dược sĩ "${pharmacist.fullName}"?`
      : `Bạn có chắc muốn mở khóa tài khoản dược sĩ "${pharmacist.fullName}"?`;

    if (!window.confirm(message))
      return;

    this.userService.updateStatus(pharmacist.id, { status: newStatus }).subscribe({
      next: () => {
        this.successMessage = newStatus === 'active'
          ? 'Mở khóa tài khoản dược sĩ thành công.'
          : 'Khóa tài khoản dược sĩ thành công.';

        this.loadPharmacists();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  deletePharmacist(pharmacist: AdminUser): void {
    const confirmed = window.confirm(
      `Vô hiệu hóa dược sĩ "${pharmacist.fullName}"? Các phiếu nhập và dữ liệu nghiệp vụ đã phát sinh vẫn được giữ lại.`
    );

    if (!confirmed)
      return;

    this.userService.delete(pharmacist.id).subscribe({
      next: response => {
        this.successMessage = response.message;
        this.loadPharmacists();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  getGenderLabel(gender: string | null): string {
    if (gender === 'male') return 'Nam';
    if (gender === 'female') return 'Nữ';
    if (gender === 'other') return 'Khác';
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

  private handleSaveSuccess(message: string): void {
    this.isSaving = false;
    this.isFormOpen = false;
    this.editingUserId = null;
    this.successMessage = message;
    this.loadPharmacists();
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
