import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard {
  private tokenService = inject(TokenService);
  private router = inject(Router);

  canActivate(): boolean {
    const token = this.tokenService.getAccessToken();
    if (token) {
      return true;
    } else {
      this.router.navigate(['/auth/sign-in']);
      return false;
    }
  }
}
