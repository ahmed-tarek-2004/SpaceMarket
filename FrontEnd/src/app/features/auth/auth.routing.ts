import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/auth-page/auth-page.component').then((m) => m.AuthPageComponent),
    data: { hideHeader: true, hideFooter: true },
  },
  {
    path: 'sign-in',
    loadComponent: () =>
      import('./pages/auth-page/auth-page.component').then((m) => m.AuthPageComponent),
    data: { hideHeader: true, hideFooter: true, tab: 'signin' },
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/auth-page/auth-page.component').then((m) => m.AuthPageComponent),
    data: { hideHeader: true, hideFooter: true, tab: 'register' },
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./pages/forget-password-page/forget-password-page.component').then(
        (m) => m.ForgetPasswordPageComponent
      ),
      data: { hideHeader: true, hideFooter: true },
  },
  {
    path: 'verify-otp',
    loadComponent: () =>
      import('./pages/verify-otp-page/verify-otp-page.component').then(
        (m) => m.VerifyOtpPageComponent
      ),
      data: { hideHeader: true, hideFooter: true },
  },
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./pages/reset-password-page/reset-password-page.component').then(
        (m) => m.ResetPasswordPageComponent
      ),
      data: { hideHeader: true, hideFooter: true },
  },
];
