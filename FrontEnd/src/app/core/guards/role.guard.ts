// role.guard.ts
import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot } from '@angular/router';
import { TokenService } from '../services/token.service';

@Injectable({
  providedIn: 'root',
})
export class RoleGuard {
  private tokenService = inject(TokenService);
  private router = inject(Router);

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRoles = route.data['roles'] as string[];
    const userRole = this.tokenService.getRole();

    if (userRole && expectedRoles.includes(userRole)) {
      return true;
    }

    // redirect if role not allowed
    this.router.navigate(['/']);
    return false;
  }
}
