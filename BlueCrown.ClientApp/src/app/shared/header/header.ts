import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, AsyncPipe],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly currentUser$ = this.authService.currentUser$;

  getDashboardPath(role: string): string {
    switch (role.trim().toLowerCase()) {
      case 'patient':
        return '/patient/dashboard';
      case 'doctor':
        return '/doctor/dashboard';
      case 'pharmacist':
        return '/pharmacist/dashboard';
      case 'admin':
        return '/admin/dashboard';
      default:
        return '/';
    }
  }

  logout(): void {
    const confirmed = window.confirm('Bạn có chắc chắn muốn đăng xuất tài khoản?');

    if (!confirmed) {
      return;
    }

    this.authService.logout();
    this.router.navigate(['/']);
  }
}
