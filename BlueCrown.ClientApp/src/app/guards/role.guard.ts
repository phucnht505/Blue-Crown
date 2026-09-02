import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const user = authService.getCurrentUser();

  if (!user) {
    return router.createUrlTree(['/login']);
  }

  const allowedRoles = (route.data['roles'] as string[] | undefined) ?? [];
  const currentRole = user.role.trim().toLowerCase();

  if (allowedRoles.map(role => role.toLowerCase()).includes(currentRole)) {
    return true;
  }

  return router.createUrlTree(['/unauthorized']);
};
