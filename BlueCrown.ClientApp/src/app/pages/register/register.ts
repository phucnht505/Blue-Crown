import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { RegisterRequest } from '../../models/auth.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  isSubmitting = false;
  errorMessage = '';
  successMessage = '';
  private returnUrl = '/';

  registerForm = this.formBuilder.nonNullable.group({
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
    confirmPassword: ['', [
      Validators.required,
    ]],
    dateOfBirth: [''],
    gender: [''],
  });

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '/';
  }

  get fullName() {
    return this.registerForm.controls.fullName;
  }

  get email() {
    return this.registerForm.controls.email;
  }

  get phone() {
    return this.registerForm.controls.phone;
  }

  get password() {
    return this.registerForm.controls.password;
  }

  get confirmPassword() {
    return this.registerForm.controls.confirmPassword;
  }

  get dateOfBirth() {
    return this.registerForm.controls.dateOfBirth;
  }

  get gender() {
    return this.registerForm.controls.gender;
  }

  get passwordsDoNotMatch(): boolean {
    return this.confirmPassword.value !== '' && this.password.value !== this.confirmPassword.value;
  }

  get dateOfBirthError(): string {
    const value = this.dateOfBirth.value;

    if (!value) {
      return '';
    }

    const birthDate = new Date(`${value}T00:00:00`);
    const today = new Date();

    if (Number.isNaN(birthDate.getTime())) {
      return 'Ngày sinh không hợp lệ.';
    }

    if (birthDate > today) {
      return 'Ngày sinh không được lớn hơn ngày hiện tại.';
    }

    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDifference = today.getMonth() - birthDate.getMonth();

    if (monthDifference < 0 || (monthDifference === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }

    if (age < 16) {
      return 'Người dùng phải từ đủ 16 tuổi.';
    }

    return '';
  }

  clearMessage(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  submit(): void {
    this.clearMessage();
    this.registerForm.markAllAsTouched();

    if (this.registerForm.invalid) {
      return;
    }

    if (this.passwordsDoNotMatch) {
      return;
    }

    if (this.dateOfBirthError) {
      return;
    }

    const formValue = this.registerForm.getRawValue();

    const request: RegisterRequest = {
      fullName: formValue.fullName.trim(),
      email: formValue.email.trim().toLowerCase(),
      phone: formValue.phone.trim(),
      password: formValue.password,
      dateOfBirth: formValue.dateOfBirth || null,
      gender: formValue.gender || null,
    };

    this.isSubmitting = true;

    this.authService.register(request).subscribe({
      next: (response) => {
        this.successMessage = response.message || 'Đăng ký thành công.';
        this.isSubmitting = false;
        this.changeDetectorRef.detectChanges();

        setTimeout(() => {
          this.router.navigate(['/login'], {
            queryParams: { returnUrl: this.returnUrl }
          });
        }, 1200);
      },

      error: (error) => {
        console.error('Lỗi đăng ký:', error);
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSubmitting = false;
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

    if (error?.error?.title && !error?.error?.errors) {
      return error.error.title;
    }

    if (error?.error?.errors) {
      const validationErrors = Object.values(error.error.errors).flat();

      if (validationErrors.length > 0) {
        return String(validationErrors[0]);
      }
    }

    return 'Không thể đăng ký tài khoản. Vui lòng thử lại.';
  }
}
