import { Component, signal, HostListener, inject, computed, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { TokenService } from '../../../core/services/token.service';
import { NotificationBellComponent } from '../notification-bell/notification-bell.component';
import { ROUTES } from '../../config/constants';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, NotificationBellComponent],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  ROUTES = ROUTES;
  private router = inject(Router);
  private authService = inject(AuthService);
  private tokenService = inject(TokenService);

  // mobile menu state
  userMenuOpen = signal(false);
  menuOpen = signal(false);

  // Authentication state
  isLoggedIn = computed(() => this.tokenService.isAuthenticated());

  // User data - will be populated from token service
  currentUser = signal<any | null>(null);

  // Service provider check
  isServiceProvider = computed(() => {
    const role = this.tokenService.getRole();
    return role === 'serviceprovider' || role === 'admin' || role === 'provider';
  });

  ngOnInit() {
    this.checkAuthStatus();

    // Listen for authentication changes
    this.tokenService.authState$.subscribe(() => {
      this.checkAuthStatus();
    });
  }

  private checkAuthStatus() {
    if (this.isLoggedIn()) {
      this.loadUserData();
    } else {
      this.currentUser.set(null);
    }
  }

  private loadUserData() {
    const userData = {
      id: this.tokenService.getUserId(),
      email: this.tokenService.getUserEmail(),
      phoneNumber: this.tokenService.getUserPhone(),
      role: this.tokenService.getRole(),
      displayName: this.tokenService.getDisplayName(),
      name: this.getUserNameFromData(),
      isEmailConfirmed: true,
    };

    this.currentUser.set(userData);
  }

  private getUserNameFromData(): string {
    // First try to use the display name
    const displayName = this.tokenService.getDisplayName();
    if (displayName && displayName.trim() !== '') {
      return displayName;
    }

    // Fallback to phone number formatting
    const phone = this.tokenService.getUserPhone();
    if (phone) {
      // Remove country code and format: +201090908451 -> 01090908451
      const formattedPhone = phone.replace(/^\+20/, '0');
      return formattedPhone;
    }

    // Otherwise use email username
    const email = this.tokenService.getUserEmail();
    if (email) {
      return email.split('@')[0];
    }

    return 'User';
  }

  toggleMenu() {
    this.menuOpen.update((v) => !v);
    if (this.userMenuOpen()) {
      this.userMenuOpen.set(false);
    }
  }

  closeMenu() {
    this.menuOpen.set(false);
  }

  toggleUserMenu() {
    this.userMenuOpen.update((v) => !v);
  }

  // User information methods
  getUserName(): string {
    return this.currentUser()?.name || this.currentUser()?.phoneNumber || 'User';
  }

  getUserEmail(): string {
    return this.currentUser()?.email || 'user@example.com';
  }

  getUserInitials(): string {
    const name = this.getUserName();

    // For display names (full names or company names), use first letters of words
    const displayName = this.tokenService.getDisplayName();
    if (displayName && displayName.trim() !== '') {
      return displayName
        .split(' ')
        .map((n) => n[0])
        .join('')
        .toUpperCase()
        .substring(0, 2);
    }

    // For phone numbers, use first digit or 'U'
    if (/^\d+$/.test(name)) {
      return name.charAt(0) || 'U';
    }

    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }

  getDashboardRoute(): string {
    const role = this.tokenService.getRole()?.toLowerCase();

    switch (role) {
      case 'admin':
        return '/admin-dashboard';
      case 'provider':
      case 'serviceprovider':
        return '/provider-dashboard';
      case 'client':
        return '/client-dashboard';
      default:
        return '/dashboard'; // fallback
    }
  }

  logout() {
    this.authService.logout();
    this.currentUser.set(null);
    this.userMenuOpen.set(false);
    this.menuOpen.set(false);
  }

  // Close menus on ESC
  @HostListener('window:keydown.escape')
  onEscape() {
    this.closeMenu();
    this.userMenuOpen.set(false);
  }

  // Close user menu when clicking outside
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event) {
    const target = event.target as HTMLElement;
    if (!target.closest('.user-menu-trigger') && !target.closest('.user-dropdown')) {
      this.userMenuOpen.set(false);
    }
  }
}
