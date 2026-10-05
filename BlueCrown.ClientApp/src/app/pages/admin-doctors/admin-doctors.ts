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
  formValidationMessage = '';

  filterForm = this.formBuilder.nonNullable.group({
    search: [''],
    specialty: [''],
    status: [''],
  });

  doctorForm = this.formBuilder.nonNullable.group({
    fullName: ['', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50),
      Validators.pattern(/^(?:BS\.\s)?[\p{L}]+(?:\s+[\p{L}]+)*$/u),
    ]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required, Validators.pattern(/^(03|05|07|08|09)\d{8}$/)]],
    password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/),
    ]],
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

    this.formValidationMessage = '';
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

        this.formValidationMessage = '';
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
    this.formValidationMessage = '';
    this.doctorForm.markAsUntouched();
  }

  closeDetail(): void {
    this.selectedDoctor = null;
  }

  submit(): void {
    this.clearMessages();
    this.doctorForm.markAllAsTouched();

    if (this.doctorForm.invalid) {
      this.formValidationMessage = 'Vui lòng kiểm tra và sửa các thông tin không hợp lệ trước khi lưu.';
      this.changeDetectorRef.detectChanges();
      return;
    }

    this.formValidationMessage = '';

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

  deleteDoctor(doctor: AdminDoctor): void {
    if (!window.confirm(
      `Bạn có chắc muốn XÓA VĨNH VIỄN tài khoản bác sĩ "${doctor.fullName}"?\n\n` +
      `Tài khoản chỉ được xóa nếu chưa có dữ liệu nghiệp vụ hoặc ràng buộc liên quan.\n` +
      `Nếu bác sĩ đã có dữ liệu, hệ thống sẽ không xóa và bạn cần khóa tài khoản thay thế.\n\n` +
      `Hành động này không thể hoàn tác.`
    ))
      return;

    this.clearMessages();

    this.doctorService.delete(doctor.userId).subscribe({
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

  getFieldError(controlName: string): string {
    const control = this.doctorForm.get(controlName);

    if (!control || !control.touched || !control.errors)
      return '';

    if (control.errors['required']) {
      switch (controlName) {
        case 'fullName':
          return 'Họ tên không được để trống.';
        case 'email':
          return 'Email không được để trống.';
        case 'phone':
          return 'Số điện thoại không được để trống.';
        case 'password':
          return 'Mật khẩu không được để trống.';
        case 'specialty':
          return 'Chuyên khoa không được để trống.';
        case 'licenseNumber':
          return 'Số giấy phép không được để trống.';
        case 'status':
          return 'Trạng thái không được để trống.';
        default:
          return 'Trường này không được để trống.';
      }
    }

    if (control.errors['minlength']) {
      const requiredLength = control.errors['minlength'].requiredLength;

      if (controlName === 'password')
        return `Mật khẩu phải có ít nhất ${requiredLength} ký tự.`;

      if (controlName === 'fullName')
        return `Họ tên phải có ít nhất ${requiredLength} ký tự.`;

      return `Phải có ít nhất ${requiredLength} ký tự.`;
    }

    if (control.errors['maxlength']) {
      const requiredLength = control.errors['maxlength'].requiredLength;

      if (controlName === 'fullName')
        return `Họ tên không được vượt quá ${requiredLength} ký tự.`;

      if (controlName === 'specialty')
        return `Chuyên khoa không được vượt quá ${requiredLength} ký tự.`;

      if (controlName === 'licenseNumber')
        return `Số giấy phép không được vượt quá ${requiredLength} ký tự.`;

      if (controlName === 'bio')
        return `Giới thiệu không được vượt quá ${requiredLength} ký tự.`;

      return `Không được vượt quá ${requiredLength} ký tự.`;
    }

    if (control.errors['email'])
      return 'Email không đúng định dạng.';

    if (control.errors['pattern']) {
      switch (controlName) {
        case 'fullName':
          return 'Họ tên chỉ được chứa chữ cái và khoảng trắng, hoặc tiền tố BS. ở đầu.';
        case 'phone':
          return 'Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 03, 05, 07, 08 hoặc 09.';
        case 'password':
          return 'Mật khẩu phải có ít nhất 1 chữ thường, 1 chữ hoa và 1 chữ số.';
        default:
          return 'Thông tin không đúng định dạng.';
      }
    }

    if (control.errors['min']) {
      return `Giá trị không được nhỏ hơn ${control.errors['min'].min}.`;
    }

    if (control.errors['max']) {
      return `Giá trị không được lớn hơn ${control.errors['max'].max}.`;
    }

    return 'Thông tin không hợp lệ.';
  }

  hasFieldError(controlName: string): boolean {
    const control = this.doctorForm.get(controlName);
    return !!control && control.touched && control.invalid;
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.formValidationMessage = '';
  }

  private toNullableNumber(value: string): number | null {
    return value === '' ? null : Number(value);
  }

  private handleSaveSuccess(message: string): void {
    this.isSaving = false;
    this.isFormOpen = false;
    this.editingId = null;
    this.formValidationMessage = '';
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
