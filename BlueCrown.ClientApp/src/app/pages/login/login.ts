import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { LoginRequest } from '../../models/auth.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  isSubmitting = false;
  errorMessage = '';
  private returnUrl = '/';

  loginForm = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  get email() {
    return this.loginForm.controls.email;
  }

  get password() {
    return this.loginForm.controls.password;
  }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '/';

    if (this.authService.isAuthenticated()) {
      this.router.navigateByUrl(this.returnUrl);
    }
  }

  clearError(): void {
    this.errorMessage = '';
  }

  submit(): void {
    this.errorMessage = '';
    this.loginForm.markAllAsTouched();

    if (this.loginForm.invalid) {
      return;
    }

    const value = this.loginForm.getRawValue();

    const request: LoginRequest = {
      email: value.email.trim().toLowerCase(),
      password: value.password,
    };

    this.isSubmitting = true;

    this.authService.login(request).subscribe({
      next: (response) => {
        this.authService.saveSession(response);
        this.isSubmitting = false;
        this.changeDetectorRef.detectChanges();
        this.router.navigateByUrl(this.returnUrl);
      },

      error: (error) => {
        console.error('Lỗi đăng nhập:', error);
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

    if (error?.error?.errors) {
      const validationErrors = Object.values(error.error.errors).flat();

      if (validationErrors.length > 0) {
        return String(validationErrors[0]);
      }
    }

    return 'Không thể đăng nhập. Vui lòng thử lại.';
  }
}
