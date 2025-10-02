import { inject, Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, switchMap, filter, take } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AuthService } from '../services/auth.service';
import { TokenService } from '../services/token.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject = new BehaviorSubject<string | null>(null);

  private authService = inject(AuthService);
  private tokenService = inject(TokenService);

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const isPublic = this.isPublicRequest(request);

    console.log('[AuthInterceptor] Intercepting:', request.url);
    console.log('[AuthInterceptor] Is public request?', isPublic);

    // Only add token if not public
    if (!isPublic) {
      const accessToken = this.tokenService.getAccessToken();
      console.log('[AuthInterceptor] Access token from TokenService:', accessToken);

      if (accessToken) {
        request = this.addTokenHeader(request, accessToken);
        console.log('[AuthInterceptor] Added Authorization header');
      } else {
        console.warn('[AuthInterceptor] No access token found!');
      }
    }

    return next.handle(request).pipe(
      catchError((error) => {
        if (
          error instanceof HttpErrorResponse &&
          error.status === 401 && // Unauthorized
          !this.isPublicRequest(request)
        ) {
          console.warn('[AuthInterceptor] 401 detected for:', request.url);
          return this.handle401Error(request, next);
        }
        console.error('[AuthInterceptor] HTTP error:', error);
        return throwError(() => error);
      })
    );
  }

  // Attach bearer token
  private addTokenHeader(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      headers: request.headers.set('Authorization', `Bearer ${token}`),
    });
  }

  // Detect public endpoints (skip them)
  private isPublicRequest(request: HttpRequest<any>): boolean {
    const publicEndpoints = [
      environment.account.clientSignup,
      environment.account.providerSignup,
      environment.account.signIn,
      environment.account.verifyOtp,
      environment.account.forgetPassword,
      environment.account.resetPassword,
      environment.account.refresh, // handled separately
    ];

    const isPublic = publicEndpoints.some((endpoint) => request.url.includes(endpoint));
    if (isPublic) {
      console.log('[AuthInterceptor] Matched public endpoint →', request.url);
    }
    return isPublic;
  }

  // Handle expired access token
  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    console.log('[AuthInterceptor] Handling 401, refreshing token...');

    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      const refreshToken = this.tokenService.getRefreshToken();
      console.log('[AuthInterceptor] Refresh token from TokenService:', refreshToken);

      if (refreshToken) {
        return this.authService.refreshToken().pipe(
          switchMap((tokens) => {
            this.isRefreshing = false;

            console.log('[AuthInterceptor] Refresh successful, new tokens:', tokens);

            // ✅ Save the new tokens
            this.tokenService.saveTokens(tokens);

            // ✅ Emit the new access token for queued requests
            this.refreshTokenSubject.next(tokens.accessToken);

            // ✅ Retry the original request
            return next.handle(this.addTokenHeader(request, tokens.accessToken));
          }),
          catchError((err) => {
            this.isRefreshing = false;
            console.error('[AuthInterceptor] Refresh failed, logging out', err);
            this.authService.logout(); // clears tokens + redirects
            return throwError(() => err);
          })
        );
      } else {
        this.isRefreshing = false;
        console.error('[AuthInterceptor] No refresh token available → logout');
        this.authService.logout();
        return throwError(() => new Error('No refresh token available'));
      }
    }

    // Queue requests while refresh is in progress
    return this.refreshTokenSubject.pipe(
      filter((token) => token !== null),
      take(1),
      switchMap((token) => {
        console.log('[AuthInterceptor] Queued request resumed with new token');
        return next.handle(this.addTokenHeader(request, token!));
      })
    );
  }
}
