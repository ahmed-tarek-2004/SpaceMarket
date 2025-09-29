import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/landing/pages/landing-page/landing-page.component').then(
        (m) => m.LandingPageComponent
      ),
  },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routing').then((m) => m.AUTH_ROUTES),
  },
 {
  path: 'service/:id',
  loadComponent: () => import('./features/service-detail/pages/service-detail-page/service-detail-page.component').then(m => m.ServiceDetailPageComponent),
  
},
  // Add other routes here
  { path: '**', redirectTo: '' },
];
