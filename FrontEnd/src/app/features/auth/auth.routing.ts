import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'sign-in',
    pathMatch: 'full'
  },
  {
    path: 'sign-in',
    loadComponent: () => 
      import('./pages/sign-in-page/sign-in-page.component')
        .then(m => m.SignInPageComponent)
  },
  {
    path: 'forgot-password',
    loadComponent: () => import('./pages/forget-password-page/forget-password-page.component').then(m => m.ForgetPasswordPageComponent)
  },
  { path: 'verify-otp', 
    loadComponent: () => import('./pages/verify-otp-page/verify-otp-page.component').then(m => m.VerifyOtpPageComponent) },
  { path: 'reset-password', 
    loadComponent: () => import('./pages/reset-password-page/reset-password-page.component').then(m => m.ResetPasswordPageComponent) },

  {
    path: 'sign-up',
    loadComponent: () => 
      import('./pages/sign-up-page/sign-up-page.component')
        .then(m => m.SignUpPageComponent)
  }
];