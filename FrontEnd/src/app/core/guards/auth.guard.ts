import { Injectable, inject } from '@angular/core';
import {
  CanActivate,
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { TokenService } from '../services/token.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  private tokenService = inject(TokenService);
  private router = inject(Router);

  canActivate(_route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    if (this.tokenService.isAuthenticated()) {
      return true;
    }

    // redirect to sign-in and preserve return url
    return this.router.createUrlTree(['/auth/sign-in'], {
      queryParams: { returnUrl: state.url },
    });
  }
}
