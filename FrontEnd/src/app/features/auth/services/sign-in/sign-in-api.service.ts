import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginRequest } from '../../interfaces/sign-in/login-request';
import { LoginResponse } from '../../interfaces/sign-in/login-response';
import { ApiResponse } from '../../../../core/interfaces/api-response';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SignInApiService {
  private apiUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${this.apiUrl}${environment.account.signIn}`,
      credentials
    );
  }

  forgetPassword(email: string): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}${environment.account.forgetPassword}`, {
      email,
    });
  }

  verifyOtp<T = any>(userId: string, otp: string): Observable<ApiResponse<T>> {
    return this.http.post<ApiResponse<T>>(`${this.apiUrl}${environment.account.verifyOtp}`, {
      userId,
      otp,
    });
  }

  resetPassword(resetData: {
    userId: string;
    otp: string;
    newPassword: string;
  }): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(
      `${this.apiUrl}${environment.account.resetPassword}`,
      resetData
    );
  }
}
