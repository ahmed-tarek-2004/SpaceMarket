// auth.guard.ts
import { inject } from '@angular/core';
import { Router } from '@angular/router';

export const authGuard = () => {
  const router = inject(Router);
  
  const isAuthenticated = !!localStorage.getItem('auth_token');

  if (isAuthenticated) {
    return true;
  } else {
    router.navigate(['/auth/sign-in']);
    return false;
  }
};