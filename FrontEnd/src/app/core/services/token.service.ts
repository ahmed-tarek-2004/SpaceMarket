import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  private readonly ACCESS_TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly ROLE_KEY = 'user_role';
  private readonly USER_ID_KEY = 'user_id';
  private readonly USER_EMAIL_KEY = 'user_email';
  private readonly USER_PHONE_KEY = 'user_phone';
  private readonly DISPLAY_NAME_KEY = 'display_name';

  // Add auth state observable
  private authState = new BehaviorSubject<boolean>(this.isAuthenticated());
  authState$ = this.authState.asObservable();

  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  getRole(): string | null {
    const r = localStorage.getItem(this.ROLE_KEY);
    return r ? r.toLowerCase().trim() : null;
  }

  getUserId(): string | null {
    return localStorage.getItem(this.USER_ID_KEY);
  }

  getUserEmail(): string | null {
    return localStorage.getItem(this.USER_EMAIL_KEY);
  }

  getUserPhone(): string | null {
    return localStorage.getItem(this.USER_PHONE_KEY);
  }

  getDisplayName(): string | null {
    return localStorage.getItem(this.DISPLAY_NAME_KEY);
  }

  saveTokens(tokens: {
    accessToken: string;
    refreshToken: string;
    role?: string;
    userId?: string;
    email?: string;
    phoneNumber?: string;
    displayName?: string;
  }): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, tokens.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, tokens.refreshToken);

    if (tokens.role) {
      localStorage.setItem(this.ROLE_KEY, tokens.role.toLowerCase().trim());
    }
    if (tokens.userId) {
      localStorage.setItem(this.USER_ID_KEY, tokens.userId);
    }
    if (tokens.email) {
      localStorage.setItem(this.USER_EMAIL_KEY, tokens.email);
    }
    if (tokens.phoneNumber) {
      localStorage.setItem(this.USER_PHONE_KEY, tokens.phoneNumber);
    }
    if (tokens.displayName) {
      localStorage.setItem(this.DISPLAY_NAME_KEY, tokens.displayName);
    }

    // Notify subscribers that auth state changed
    this.authState.next(true);
  }

  clearTokens(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.ROLE_KEY);
    localStorage.removeItem(this.USER_ID_KEY);
    localStorage.removeItem(this.USER_EMAIL_KEY);
    localStorage.removeItem(this.USER_PHONE_KEY);
    localStorage.removeItem(this.DISPLAY_NAME_KEY);

    // Notify subscribers that auth state changed
    this.authState.next(false);
  }

  isAuthenticated(): boolean {
    return !!this.getAccessToken();
  }
}
