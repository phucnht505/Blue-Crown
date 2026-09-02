import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminUser, AdminUserDetail, CreateAdminUserRequest, UpdateAdminUserRequest } from '../../models/admin-user.model';
import { AdminUserService } from '../../services/admin-user.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './admin-users.html',
  styleUrl: './admin-users.css',
})
export class AdminUsers implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly userService = inject(AdminUserService);
  private readonly authService = inject(AuthService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  users: AdminUser[] = [];
  selectedUser: AdminUserDetail | null = null;
  editingUserId: string | null = null;
  currentAdminId = '';
  isLoading = false;
  isSaving = false;
  isFormOpen = false;
  errorMessage = '';
  successMessage = '';

  filterForm = this.formBuilder.nonNullable.group({
    search: [''],
    role: [''],
    status: [''],
  });

  userForm = this.formBuilder.nonNullable.group({
    fullName: ['', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50),
      Validators.pattern(/^[\p{L}\s]+$/u),
    ]],
    email: ['', [
      Validators.required,
      Validators.email,
    ]],
    phone: ['', [
      Validators.required,
      Validators.pattern(/^(03|05|07|08|09)\d{8}$/),
    ]],
    password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/),
    ]],
    dateOfBirth: [''],
    gender: [''],
    avatarUrl: [''],
    role: ['patient', Validators.required],
    status: ['active', Validators.required],
  });

  ngOnInit(): void {
    this.currentAdminId = this.authService.getCurrentUser()?.userId ?? '';
    this.loadUsers();
  }

  loadUsers(): void {
    const filter = this.filterForm.getRawValue();

    this.isLoading = true;
    this.errorMessage = '';

    this.userService.getAll(filter.search, filter.role, filter.status).subscribe({
      next: users => {
        this.users = users;
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
      role: '',
      status: '',
    });

    this.loadUsers();
  }

  openCreate(): void {
    this.clearMessages();
    this.editingUserId = null;
    this.selectedUser = null;
    this.isFormOpen = true;

    this.userForm.controls.password.setValidators([
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/),
    ]);
    this.userForm.controls.password.updateValueAndValidity();

    this.userForm.reset({
      fullName: '',
      email: '',
      phone: '',
      password: '',
      dateOfBirth: '',
      gender: '',
      avatarUrl: '',
      role: 'patient',
      status: 'active',
    });
  }

  openEdit(id: string): void {
    this.clearMessages();

    this.userService.getById(id).subscribe({
      next: user => {
        this.editingUserId = id;
        this.selectedUser = user;
        this.isFormOpen = true;

        this.userForm.controls.password.clearValidators();
        this.userForm.controls.password.updateValueAndValidity();

        this.userForm.reset({
          fullName: user.fullName ?? '',
          email: user.email ?? '',
          phone: user.phone ?? '',
          password: '',
          dateOfBirth: user.dateOfBirth ?? '',
          gender: user.gender ?? '',
          avatarUrl: user.avatarUrl ?? '',
          role: user.role ?? 'patient',
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
        this.selectedUser = user;
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
    this.selectedUser = null;
  }

  submit(): void {
    this.clearMessages();
    this.userForm.markAllAsTouched();

    if (this.userForm.invalid)
      return;

    const value = this.userForm.getRawValue();
    this.isSaving = true;

    if (!this.editingUserId) {
      const request: CreateAdminUserRequest = {
        fullName: value.fullName.trim(),
        email: value.email.trim().toLowerCase(),
        phone: value.phone.trim(),
        password: value.password,
        dateOfBirth: value.dateOfBirth || null,
        gender: value.gender || null,
        role: value.role,
        status: value.status,
      };

      this.userService.create(request).subscribe({
        next: () => this.handleSaveSuccess('Thêm tài khoản thành công.'),
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
      role: value.role,
      status: value.status,
    };

    this.userService.update(this.editingUserId, request).subscribe({
      next: () => this.handleSaveSuccess('Cập nhật tài khoản thành công.'),
      error: error => this.handleSaveError(error),
    });
  }

  toggleStatus(user: AdminUser): void {
    if (user.id === this.currentAdminId)
      return;

    const newStatus = user.status === 'active' ? 'suspended' : 'active';

    const message = newStatus === 'suspended'
      ? 'Bạn có chắc muốn khóa tài khoản này?'
      : 'Bạn có chắc muốn mở khóa tài khoản này?';

    if (!window.confirm(message))
      return;

    this.clearMessages();

    this.userService.updateStatus(user.id, { status: newStatus }).subscribe({
      next: () => {
        this.successMessage = newStatus === 'active'
          ? 'Mở khóa tài khoản thành công.'
          : 'Khóa tài khoản thành công.';

        this.loadUsers();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  deleteUser(user: AdminUser): void {
    if (user.id === this.currentAdminId)
      return;

    const confirmed = window.confirm(
      `Vô hiệu hóa tài khoản "${user.fullName}"? Dữ liệu liên quan sẽ được giữ lại.`
    );

    if (!confirmed)
      return;

    this.clearMessages();

    this.userService.delete(user.id).subscribe({
      next: response => {
        this.successMessage = response.message;
        this.loadUsers();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  getRoleLabel(role: string | null): string {
    switch (role) {
      case 'admin':
        return 'Admin';
      case 'doctor':
        return 'Bác sĩ';
      case 'pharmacist':
        return 'Dược sĩ';
      case 'patient':
        return 'Bệnh nhân';
      default:
        return role || '-';
    }
  }

  getStatusLabel(status: string | null): string {
    switch (status) {
      case 'active':
        return 'Hoạt động';
      case 'suspended':
        return 'Đã khóa';
      case 'pending':
        return 'Chờ kích hoạt';
      default:
        return '-';
    }
  }

  getGenderLabel(gender: string | null): string {
    switch (gender) {
      case 'male':
        return 'Nam';
      case 'female':
        return 'Nữ';
      case 'other':
        return 'Khác';
      default:
        return '-';
    }
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
    this.loadUsers();
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
