import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ForgotPasswordRequest, ResetPasswordRequest } from '../../models/auth.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css',
})
export class ForgotPassword {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  otpSent = false;
  isSendingOtp = false;
  isResetting = false;
  errorMessage = '';
  successMessage = '';

  forgotPasswordForm = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    otp: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
    newPassword: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(100)]],
    confirmPassword: ['', Validators.required],
  });

  get email() {
    return this.forgotPasswordForm.controls.email;
  }

  get otp() {
    return this.forgotPasswordForm.controls.otp;
  }

  get newPassword() {
    return this.forgotPasswordForm.controls.newPassword;
  }

  get confirmPassword() {
    return this.forgotPasswordForm.controls.confirmPassword;
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  sendOtp(): void {
    this.clearMessages();
    this.email.markAsTouched();

    if (this.email.invalid) return;

    const request: ForgotPasswordRequest = {
      email: this.email.value.trim().toLowerCase(),
    };

    this.isSendingOtp = true;

    this.authService.forgotPassword(request).subscribe({
      next: response => {
        this.otpSent = true;
        this.successMessage = response.message;
        this.isSendingOtp = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSendingOtp = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  resetPassword(): void {
    this.clearMessages();

    this.otp.markAsTouched();
    this.newPassword.markAsTouched();
    this.confirmPassword.markAsTouched();

    if (this.otp.invalid || this.newPassword.invalid || this.confirmPassword.invalid) return;

    if (this.newPassword.value !== this.confirmPassword.value) {
      this.errorMessage = 'Mật khẩu xác nhận không khớp.';
      return;
    }

    const request: ResetPasswordRequest = {
      email: this.email.value.trim().toLowerCase(),
      otp: this.otp.value.trim(),
      newPassword: this.newPassword.value,
      confirmPassword: this.confirmPassword.value,
    };

    this.isResetting = true;

    this.authService.resetPassword(request).subscribe({
      next: response => {
        this.successMessage = `${response.message} Đang chuyển về trang đăng nhập...`;
        this.isResetting = false;
        this.changeDetectorRef.detectChanges();

        window.setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1200);
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isResetting = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') return error.error;
    if (error?.error?.message) return error.error.message;

    if (error?.error?.errors) {
      const errors = Object.values(error.error.errors).flat();
      if (errors.length > 0) return String(errors[0]);
    }

    return 'Không thể xử lý yêu cầu. Vui lòng thử lại.';
  }
}
