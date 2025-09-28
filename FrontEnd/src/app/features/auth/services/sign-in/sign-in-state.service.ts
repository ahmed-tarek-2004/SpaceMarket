// src/app/features/auth/services/auth-state.service.ts
import { Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class SignInStateService {
  private _isLoading = signal(false);
  private _error = signal<string | null>(null);
  private _user = signal<any | null>(null);

  readonly isLoading = this._isLoading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly user = this._user.asReadonly();

  setLoading(value: boolean) { this._isLoading.set(value); }
  setError(message: string | null) { this._error.set(message); }
  setUser(user: any | null) { this._user.set(user); }

  saveAuthData(token: string, user: any) {
    localStorage.setItem('auth_token', token);
    this.setUser(user);
  }

  clearAuthData() {
    localStorage.removeItem('auth_token');
    this.setUser(null);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('auth_token');
  }
}