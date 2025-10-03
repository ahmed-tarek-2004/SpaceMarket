import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';
import { ForbiddenComponent } from './shared/components/forbidden/forbidden.component';

export const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
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
    loadComponent: () =>
      import(
        './features/service-detail/pages/service-detail-page/service-detail-page.component'
      ).then((m) => m.ServiceDetailPageComponent),
  },
  {
    path: 'dataset/:id',
    loadComponent: () =>
      import(
        './features/dataset-details/pages/service-detail-page/dataset-detail-page.component'
      ).then((m) => m.DatasetDetailPageComponent),
  },
  {
    path: 'create-service',
    loadComponent: () =>
      import(
        './features/create-services/pages/create-service-page/create-service-page.component'
      ).then((m) => m.CreateServicePageComponent),
    canActivate: [RoleGuard],
    data: { roles: ['serviceprovider'] },
  },
  {
    path: 'categories',
    loadComponent: () =>
      import(
        './features/dashboard/admin/components/service-category/pages/category-management/category-management.component'
      ).then((m) => m.CategoryManagementPageComponent),
    canActivate: [RoleGuard],
    data: { roles: ['admin'] },
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
    canActivate: [RoleGuard],
    data: { roles: ['client'] },
  },
  {
    path: 'admin-dashboard',
    loadComponent: () =>
      import(
        './features/dashboard/admin/pages/admin-dashboard-page/admin-dashboard-page.component'
      ).then((m) => m.AdminDashboardPageComponent),
    canActivate: [RoleGuard],
    data: { roles: ['admin'] },
  },
  {
    path: 'provider-dashboard',
    loadComponent: () =>
      import(
        './features/dashboard/provider/pages/provider-dashboard-page/provider-dashboard-page.component'
      ).then((m) => m.ProviderDashboardPageComponent),
    canActivate: [RoleGuard],
    data: { roles: ['serviceprovider'] },
  },
  {
    path: 'client-dashboard',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['client'] },
    loadComponent: () =>
      import('./features/debris-tracking/pages/satellites-page/satellites-page').then(
        (m) => m.SatellitesPage
      ),
  },
  {
    path: 'all-satellites',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['client'] },
    loadComponent: () =>
      import(
        './features/debris-tracking/pages/all-satellites-page/all-satellites-page.component'
      ).then((m) => m.AllSatellitesPageComponent),
  },
  {
    path: 'coming-soon',
    loadComponent: () =>
      import('./shared/components/coming-soon/coming-soon.component').then(
        (m) => m.ComingSoonComponent
      ),
  },
  {
    path: 'success',
    loadComponent: () =>
      import('./shared/components/success/success.component').then((m) => m.SuccessComponent),
  },
  {
    path: 'error',
    loadComponent: () =>
      import('./shared/components/error/error.component').then((m) => m.ErrorComponent),
  },
  { path: 'forbidden', component: ForbiddenComponent },
  // Add other routes here
  { path: '**', redirectTo: '' },
];
