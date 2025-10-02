import { Injectable, inject } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
  UrlTree,
} from '@angular/router';
import { TokenService } from '../services/token.service';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
  private tokenService = inject(TokenService);
  private router = inject(Router);

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    // ensure user is authenticated first
    if (!this.tokenService.isAuthenticated()) {
      return this.router.createUrlTree(['/auth/sign-in'], {
        queryParams: { returnUrl: state.url },
      });
    }

    const expectedRoles = (route.data['roles'] as string[] | undefined) ?? [];
    // normalize expected roles for robust comparison
    const expected = expectedRoles.map((r) => r?.toLowerCase().trim());

    const userRole = this.tokenService.getRole()?.toLowerCase().trim() ?? null;

    // if route has no role restriction, allow
    if (expected.length === 0) {
      return true;
    }

    if (userRole && expected.includes(userRole)) {
      return true;
    }
    if (!this.tokenService.isAuthenticated()) {
      return this.router.createUrlTree(['/auth/sign-in'], {
        queryParams: { returnUrl: state.url },
      });
    }

    if (expected.length === 0) return true;

    if (userRole && expected.includes(userRole)) return true;

    // authenticated but not authorized -> go to /forbidden with reason
    return this.router.createUrlTree(['/forbidden'], {
      queryParams: { reason: 'role', message: 'Your account does not have the required role.' },
    });
  }
}
