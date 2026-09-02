import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AccountProfile, ClinicOption, DoctorSelfProfile } from '../../models/account-profile.model';
import { AccountProfileService } from '../../services/account-profile.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-account-profile',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './account-profile.html',
  styleUrl: './account-profile.css',
})
export class AccountProfilePage implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly profileService = inject(AccountProfileService);
  private readonly authService = inject(AuthService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  profile: AccountProfile | null = null;
  doctorProfile: DoctorSelfProfile | null = null;
  clinics: ClinicOption[] = [];
  isLoading = false;
  isSavingAccount = false;
  isSavingDoctor = false;
  errorMessage = '';
  successMessage = '';
  doctorErrorMessage = '';
  doctorSuccessMessage = '';

  accountForm = this.formBuilder.nonNullable.group({
    fullName: ['', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50),
      Validators.pattern(/^[\p{L}\s]+$/u),
    ]],
    phone: ['', [
      Validators.required,
      Validators.pattern(/^(03|05|07|08|09)\d{8}$/),
    ]],
    dateOfBirth: [''],
    gender: [''],
    avatarUrl: [''],
  });

  doctorForm = this.formBuilder.nonNullable.group({
    specialty: ['', [Validators.required, Validators.maxLength(100)]],
    bio: ['', Validators.maxLength(2000)],
    yearsExperience: [''],
    clinicId: [''],
    consultationFee: [''],
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  get isDoctor(): boolean {
    return this.profile?.role?.toLowerCase() === 'doctor';
  }

  saveAccount(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.accountForm.markAllAsTouched();

    if (this.accountForm.invalid)
      return;

    const value = this.accountForm.getRawValue();

    if (value.dateOfBirth) {
      const selectedDate = new Date(`${value.dateOfBirth}T00:00:00`);
      const today = new Date();

      if (selectedDate.getTime() > today.getTime()) {
        this.errorMessage = 'Ngày sinh không hợp lệ.';
        return;
      }
    }

    this.isSavingAccount = true;

    this.profileService.updateMyProfile({
      fullName: value.fullName.trim(),
      phone: value.phone.trim(),
      dateOfBirth: value.dateOfBirth || null,
      gender: value.gender || null,
      avatarUrl: value.avatarUrl.trim() || null,
    }).subscribe({
      next: profile => {
        this.profile = profile;
        this.accountForm.patchValue({
          fullName: profile.fullName ?? '',
          phone: profile.phone ?? '',
          dateOfBirth: profile.dateOfBirth ?? '',
          gender: profile.gender ?? '',
          avatarUrl: profile.avatarUrl ?? '',
        });

        this.authService.updateCurrentUserProfile(profile.fullName);
        this.successMessage = 'Cập nhật hồ sơ cá nhân thành công.';
        this.isSavingAccount = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSavingAccount = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  saveDoctorProfile(): void {
    this.doctorErrorMessage = '';
    this.doctorSuccessMessage = '';
    this.doctorForm.markAllAsTouched();

    if (!this.doctorProfile || this.doctorForm.invalid)
      return;

    const value = this.doctorForm.getRawValue();
    const yearsExperience = this.toNullableNumber(value.yearsExperience);
    const consultationFee = this.toNullableNumber(value.consultationFee);

    if (yearsExperience !== null && (yearsExperience < 0 || yearsExperience > 80)) {
      this.doctorErrorMessage = 'Số năm kinh nghiệm phải từ 0 đến 80.';
      return;
    }

    if (consultationFee !== null && consultationFee < 0) {
      this.doctorErrorMessage = 'Phí khám không được nhỏ hơn 0.';
      return;
    }

    this.isSavingDoctor = true;

    this.profileService.updateDoctorProfile(this.doctorProfile.id, {
      specialty: value.specialty.trim(),
      bio: value.bio.trim() || null,
      yearsExperience,
      clinicId: value.clinicId || null,
      consultationFee,
    }).subscribe({
      next: () => {
        this.doctorSuccessMessage = 'Cập nhật hồ sơ chuyên môn thành công.';
        this.isSavingDoctor = false;
        this.loadDoctorProfile();
      },
      error: error => {
        this.doctorErrorMessage = this.getApiErrorMessage(error);
        this.isSavingDoctor = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  getRoleLabel(role: string | null): string {
    switch (role?.toLowerCase()) {
      case 'patient': return 'Bệnh nhân';
      case 'doctor': return 'Bác sĩ';
      case 'pharmacist': return 'Dược sĩ';
      case 'admin': return 'Quản trị viên';
      default: return role || '-';
    }
  }

  getStatusLabel(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'active': return 'Hoạt động';
      case 'suspended': return 'Đã khóa';
      case 'pending': return 'Chờ kích hoạt';
      default: return status || '-';
    }
  }

  getGenderLabel(gender: string | null): string {
    switch (gender?.toLowerCase()) {
      case 'male': return 'Nam';
      case 'female': return 'Nữ';
      case 'other': return 'Khác';
      default: return '-';
    }
  }

  private loadProfile(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.profileService.getMyProfile().subscribe({
      next: profile => {
        this.profile = profile;

        this.accountForm.patchValue({
          fullName: profile.fullName ?? '',
          phone: profile.phone ?? '',
          dateOfBirth: profile.dateOfBirth ?? '',
          gender: profile.gender ?? '',
          avatarUrl: profile.avatarUrl ?? '',
        });

        if (profile.role?.toLowerCase() === 'doctor') {
          this.loadClinics();
          this.loadDoctorProfile();
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

  private loadDoctorProfile(): void {
    this.profileService.getMyDoctorProfile().subscribe({
      next: profile => {
        this.doctorProfile = profile;

        this.doctorForm.patchValue({
          specialty: profile.specialty ?? '',
          bio: profile.bio ?? '',
          yearsExperience: profile.yearsExperience == null ? '' : String(profile.yearsExperience),
          clinicId: profile.clinicId ?? '',
          consultationFee: profile.consultationFee == null ? '' : String(profile.consultationFee),
        });

        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.doctorErrorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private loadClinics(): void {
    this.profileService.getClinics().subscribe({
      next: clinics => {
        this.clinics = clinics;
        this.changeDetectorRef.detectChanges();
      },
      error: () => {
        this.clinics = [];
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private toNullableNumber(value: string): number | null {
    if (!value.trim())
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
