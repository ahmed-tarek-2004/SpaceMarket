import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { TokenService } from './token.service';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private tokenService = inject(TokenService);

  refreshToken(): Observable<{ accessToken: string; refreshToken: string }> {
    const refreshToken = this.tokenService.getRefreshToken();
    return this.http.post<{ accessToken: string; refreshToken: string }>(
      `${environment.apiUrl}${environment.account.refresh}`,
      { refreshToken }
    );
  }

  logout(): void {
    this.tokenService.clearTokens();
    this.router.navigate(['/auth/sign-in']);
  }
}
