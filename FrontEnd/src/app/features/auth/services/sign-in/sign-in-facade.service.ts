import { Injectable, inject } from '@angular/core';
import { catchError, of, tap } from 'rxjs';
import { SignInApiService } from './sign-in-api.service';
import { SignInStateService } from './sign-in-state.service'; 
import { Router } from '@angular/router';
import { LoginRequest } from '../../interfaces/sign-in/login-request';


@Injectable({
  providedIn: 'root'
})
export class SignInFacadeService {
  private api = inject(SignInApiService) as SignInApiService;
  private state = inject(SignInStateService) as SignInStateService;
  private router=inject(Router);
   request:LoginRequest={}as LoginRequest

  login(email: string, password: string) {
    this.state.setLoading(true);
    this.state.setError(null);
    this.request.email=email
    this.request.password=password
    return this.api.login(this.request).pipe(
      tap(response => {
if(response.succeeded) {
        this.router.navigate(['/auth/verify-otp'], {queryParams: {id: response.data.id}})
      }
      else{
        console.error(response.message)
      }         this.state.setLoading(false);
      }),
      catchError(error => {
        this.state.setLoading(false);
       let message = error.error?.message || 'Invalid email or password';
       if (message.includes('Password must be at least 8 characters')) {
       message = 'Password must be at least 8 characters and include uppercase, lowercase, number, and special character.';
}
this.state.setError(message);
        return of(null);
      })
    );
  }

  forgetPassword(email: string) {
    this.state.setLoading(true);
    this.state.setError(null);
    return this.api.forgetPassword(email).pipe(
      tap(() => this.state.setLoading(false)),
      catchError(err => {
        this.state.setLoading(false);
        const msg = err.error?.message || 'Failed to send reset instructions';
        this.state.setError(msg);
        return of(null);
      })
    );
  }

  verifyOtp(otp: string) {
    this.state.setLoading(true);
    this.state.setError(null);
    return this.api.verifyOtp(otp).pipe(
      tap(() => this.state.setLoading(false)),
      catchError(err => {
        this.state.setLoading(false);
        const msg = err.error?.message || 'Invalid OTP';
        this.state.setError(msg);
        return of(null);
      })
    );
  }

  resetPassword(otp: string, newPassword: string) {
    this.state.setLoading(true);
    this.state.setError(null);
    return this.api.resetPassword(otp, newPassword).pipe(
      tap(() => this.state.setLoading(false)),
      catchError(err => {
        this.state.setLoading(false);
        const msg = err.error?.message || 'Failed to reset password';
        this.state.setError(msg);
        return of(null);
      })
    );
  }

  getIsLoading() { return this.state.isLoading(); }
  getError() { return this.state.error(); }

}