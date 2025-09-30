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
    path: 'marketplace',
    loadComponent: () =>
      import('./features/marketplace/pages/marketplace-page/marketplace-page.component').then(
        (m) => m.MarketplacePageComponent
      ),
  },
  {
    path: 'cart',
    loadComponent: () =>
      import('./features/cart/pages/cart-page/cart-page.component').then(
        (m) => m.CartPageComponent
      ),
  },
  // Add other routes here
  { path: '**', redirectTo: '' },
];
