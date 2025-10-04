import { Injectable, inject } from '@angular/core';
import { catchError, of, tap, throwError } from 'rxjs';
import { SignInApiService } from './sign-in-api.service';
import { SignInStateService } from './sign-in-state.service';
import { Router } from '@angular/router';
import { LoginRequest } from '../../interfaces/sign-in/login-request';
import { LoginResponse } from '../../interfaces/sign-in/login-response';
import { TokenService } from '../../../../core/services/token.service';

@Injectable({
  providedIn: 'root',
})
export class SignInFacadeService {
  private api = inject(SignInApiService) as SignInApiService;
  private state = inject(SignInStateService) as SignInStateService;
  private router = inject(Router);
  private tokenService = inject(TokenService);

  request: LoginRequest = {} as LoginRequest;
  private currentUserId: string | null = null;
  private currentEmail: string | null = null;

  // Initial login without OTP - triggers OTP email
  login(email: string, password: string) {
    this.state.setLoading(true);
    this.request = {
      email: email,
      password: password,
      otp: '', // No OTP in initial request
    };

    this.currentEmail = email;

    return this.api.login(this.request).pipe(
      tap((response) => {
        this.state.setLoading(false);

        if (!response) {
          throw new Error('Empty response from server');
        }

        if (response.succeeded) {
          const data = response.data as any;

          // CASE A: Backend indicates OTP was sent (no tokens, just user ID)
          if (data && data.id && !data.accessToken) {
            this.currentUserId = data.id;
            console.log('OTP sent to email. UserId:', this.currentUserId);

            // Navigate to OTP verification page
            this.router.navigate(['/auth/verify-otp'], {
              queryParams: {
                userId: this.currentUserId,
                email: this.currentEmail,
                context: 'login', // Differentiate from registration OTP
              },
            });
            return;
          }

          // CASE B: Backend returned tokens immediately
          if (data && data.accessToken) {
            this.tokenService.saveTokens({
              accessToken: data.accessToken,
              refreshToken: data.refreshToken,
              role: data.role,
              userId: data.id,
              email: data.email,
              phoneNumber: data.phoneNumber,
              displayName: data.displayName,
            });

            this.currentUserId = data.id;
            console.log('Login successful with OTP, navigating home');

            this.router.navigate(['/']);
            return;
          }

          // Unexpected response
          throw new Error(response.message || 'Login responded with unexpected payload');
        }

        // API returned succeeded: false
        throw new Error(response.message || 'Login failed');
      }),
      catchError((error) => {
        this.state.setLoading(false);
        return throwError(() => error);
      })
    );
  }

  // Verify OTP for login - uses the same login endpoint but with OTP
  verifyOtpForLogin(otp: string, userId?: string) {
    this.state.setLoading(true);
    this.state.setError(null);

    const verifyUserId = userId || this.currentUserId;
    if (!verifyUserId || !this.currentEmail) {
      this.state.setLoading(false);
      throw new Error('User credentials not found for OTP verification');
    }

    console.log('Verifying OTP for login, user:', verifyUserId);

    // Create login request with OTP
    const loginRequest: LoginRequest = {
      email: this.currentEmail,
      password: this.request.password, // Use stored password
      otp: otp,
    };

    return this.api.login(loginRequest).pipe(
      tap((response) => {
        this.state.setLoading(false);

        if (response && response.succeeded && response.data) {
          const data = response.data as LoginResponse;

          if (data.accessToken && data.refreshToken) {
            this.tokenService.saveTokens({
              accessToken: data.accessToken,
              refreshToken: data.refreshToken,
              role: data.role,
              userId: data.id,
              email: data.email,
              phoneNumber: data.phoneNumber,
              displayName: data.displayName,
            });

            this.currentUserId = data.id ?? verifyUserId;

            console.log('OTP verified and tokens stored for user:', this.currentUserId);

            // Navigate to home/dashboard
            this.router.navigate(['/']);
            return;
          }

          throw new Error(response.message || 'OTP verified but tokens were not returned');
        } else {
          throw new Error(response?.message || 'OTP verification failed');
        }
      }),
      catchError((err) => {
        this.state.setLoading(false);
        const msg = (err as any)?.error?.message || (err as any)?.message || 'Invalid OTP';
        this.state.setError(msg);
        return throwError(() => err);
      })
    );
  }

  private currentOtp: string | null = null;

  // Add this method to your existing SignInFacadeService
  verifyOtpForResetPassword(userId: string, otp: string) {
    this.state.setLoading(true);
    this.state.setError(null);

    return this.api.verifyOtp<{ success: boolean }>(userId, otp).pipe(
      tap((response) => {
        this.state.setLoading(false);
        if (response && response.succeeded) {
          console.log('OTP verified for password reset');
          // Store the OTP for reset password flow
          this.currentOtp = otp;
        } else {
          throw new Error(response?.message || 'OTP verification failed');
        }
      }),
      catchError((err) => {
        this.state.setLoading(false);
        const msg = (err as any)?.error?.message || (err as any)?.message || 'Invalid OTP';
        this.state.setError(msg);
        return throwError(() => err);
      })
    );
  }

  // Verify OTP for email confirmation (registration flow)
  verifyOtpForRegistration(userId: string, otp: string) {
    this.state.setLoading(true);
    this.state.setError(null);

    return this.api.verifyOtp<{ success: boolean }>(userId, otp).pipe(
      tap((response) => {
        this.state.setLoading(false);

        if (response && response.succeeded) {
          console.log('Email verified successfully');
          // Navigate to login or success page
          this.router.navigate(['/auth/login'], {
            queryParams: { verified: true },
          });
          return;
        }

        throw new Error(response?.message || 'OTP verification failed');
      }),
      catchError((err) => {
        this.state.setLoading(false);
        const msg = (err as any)?.error?.message || (err as any)?.message || 'Invalid OTP';
        this.state.setError(msg);
        return throwError(() => err);
      })
    );
  }

  forgetPassword(email: string) {
    this.state.setLoading(true);
    this.state.setError(null);
    return this.api.forgetPassword(email).pipe(
      tap((response) => {
        this.state.setLoading(false);
        if (response && response.succeeded) {
          // Store userId for reset password
          this.currentUserId = response.data?.userId;
          console.log('Password reset instructions sent');
        } else {
          throw new Error(response?.message || 'Failed to send reset instructions');
        }
      }),
      catchError((err) => {
        this.state.setLoading(false);
        const msg = (err as any)?.error?.message || 'Failed to send reset instructions';
        this.state.setError(msg);
        return throwError(() => err);
      })
    );
  }

  // Update the resetPassword method to use stored OTP if not provided
  resetPassword(otp: string, newPassword: string) {
    this.state.setLoading(true);
    this.state.setError(null);

    // Use provided OTP or fall back to stored OTP
    const resetOtp = otp || this.currentOtp;

    if (!this.currentUserId || !resetOtp) {
      this.state.setLoading(false);
      throw new Error('User ID or OTP not found. Please request password reset again.');
    }

    const resetData = {
      userId: this.currentUserId,
      otp: resetOtp,
      newPassword: newPassword,
    };

    return this.api.resetPassword(resetData).pipe(
      tap((response) => {
        this.state.setLoading(false);
        if (response && response.succeeded) {
          console.log('Password reset successfully');
          // Clear stored OTP after successful reset
          this.currentOtp = null;
          this.router.navigate(['/auth/login'], {
            queryParams: { reset: true },
          });
        } else {
          throw new Error(response?.message || 'Failed to reset password');
        }
      }),
      catchError((err) => {
        this.state.setLoading(false);
        const msg = (err as any)?.error?.message || 'Failed to reset password';
        this.state.setError(msg);
        return throwError(() => err);
      })
    );
  }

  getIsLoading() {
    return this.state.isLoading();
  }

  getError() {
    return this.state.error();
  }

  getCurrentUserId(): string | null {
    return this.currentUserId;
  }

  // Clear stored credentials (useful for logout)
  clearStoredCredentials() {
    this.currentUserId = null;
    this.currentEmail = null;
    this.request = {} as LoginRequest;
  }
}
