import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'auth',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadComponent: () => 
      import('./pages/auth-page/auth-page.component')
        .then(m => m.AuthPageComponent)
  }
];