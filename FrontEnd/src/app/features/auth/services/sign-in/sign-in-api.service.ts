import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginRequest } from '../../interfaces/sign-in/login-request';
import { LoginResponse } from '../../interfaces/sign-in/login-response';
import { ApiResponse } from '../../../../core/interfaces/object-api-response';




@Injectable({
  providedIn: 'root'
})
export class SignInApiService {
  private apiUrl = 'https://spacemarket.runasp.net/api/Account';

  constructor(private http: HttpClient) {}

  login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.apiUrl}/login`, credentials);
  }

  forgetPassword(email: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/forget-password`, { email });
  }

  verifyOtp(otp: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/verify-otp`, { otp });
  }

  resetPassword(otp: string, newPassword: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/reset-password`, { otp, newPassword });
  }
}