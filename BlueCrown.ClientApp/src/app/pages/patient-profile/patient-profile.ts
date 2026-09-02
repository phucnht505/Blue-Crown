import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PatientProfileRequest } from '../../models/patient-profile.model';
import { PatientProfileService } from '../../services/patient-profile.service';

@Component({
  selector: 'app-patient-profile',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './patient-profile.html',
  styleUrl: './patient-profile.css',
})
export class PatientProfilePage implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly patientProfileService = inject(PatientProfileService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  isLoading = true;
  isSaving = false;
  hasProfile = false;
  errorMessage = '';
  successMessage = '';

  profileForm = this.formBuilder.nonNullable.group({
    bloodType: ['', Validators.required],
    heightCm: [0, [Validators.required, Validators.min(50), Validators.max(250)]],
    weightKg: [0, [Validators.required, Validators.min(2), Validators.max(300)]],
    allergies: ['', Validators.maxLength(500)],
    chronicConditions: ['', Validators.maxLength(500)],
    emergencyContactName: ['', Validators.maxLength(100)],
    emergencyContactPhone: ['', Validators.pattern(/^(03|05|07|08|09)\d{8}$/)],
  });

  get bloodType() {
    return this.profileForm.controls.bloodType;
  }

  get heightCm() {
    return this.profileForm.controls.heightCm;
  }

  get weightKg() {
    return this.profileForm.controls.weightKg;
  }

  get emergencyContactPhone() {
    return this.profileForm.controls.emergencyContactPhone;
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  save(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.profileForm.markAllAsTouched();

    if (this.profileForm.invalid) {
      return;
    }

    const value = this.profileForm.getRawValue();

    const request: PatientProfileRequest = {
      bloodType: value.bloodType,
      heightCm: value.heightCm,
      weightKg: value.weightKg,
      allergies: this.toNullable(value.allergies),
      chronicConditions: this.toNullable(value.chronicConditions),
      emergencyContactName: this.toNullable(value.emergencyContactName),
      emergencyContactPhone: this.toNullable(value.emergencyContactPhone),
    };

    this.isSaving = true;

    const operation = this.hasProfile
      ? this.patientProfileService.update(request)
      : this.patientProfileService.create(request);

    operation.subscribe({
      next: (response) => {
        this.hasProfile = true;
        this.successMessage = response.message;
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        console.error('Lỗi lưu hồ sơ sức khỏe:', error);
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

  private loadProfile(): void {
    this.patientProfileService.getMyProfile().subscribe({
      next: (profile) => {
        this.hasProfile = true;

        this.profileForm.patchValue({
          bloodType: profile.bloodType,
          heightCm: profile.heightCm ?? 0,
          weightKg: profile.weightKg ?? 0,
          allergies: profile.allergies ?? '',
          chronicConditions: profile.chronicConditions ?? '',
          emergencyContactName: profile.emergencyContactName ?? '',
          emergencyContactPhone: profile.emergencyContactPhone ?? '',
        });

        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        if (error.status === 404) {
          this.hasProfile = false;
          this.isLoading = false;
          this.changeDetectorRef.detectChanges();
          return;
        }

        console.error('Lỗi tải hồ sơ sức khỏe:', error);
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private toNullable(value: string): string | null {
    const normalized = value.trim();
    return normalized || null;
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') {
      return error.error;
    }

    if (error?.error?.message) {
      return error.error.message;
    }

    if (error?.error?.errors) {
      const validationErrors = Object.values(error.error.errors).flat();

      if (validationErrors.length > 0) {
        return String(validationErrors[0]);
      }
    }

    return 'Không thể xử lý hồ sơ sức khỏe. Vui lòng thử lại.';
  }
}
