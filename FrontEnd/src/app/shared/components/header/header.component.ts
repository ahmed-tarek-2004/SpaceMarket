import { Component, signal, HostListener, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { User } from '../../../core/interfaces/user';
import { ROUTES } from '../../config/constants';

@Component({
  selector: 'app-header',
  imports: [ RouterLink ],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent {
  ROUTES = ROUTES
  private router = inject(Router);
  
  // mobile menu state
  userMenuOpen = signal(false);
  menuOpen = signal(false);

  // Mock user data - replace with actual authentication service
  currentUser = signal<User | null>(null);
  isLoggedIn = signal(false);

  constructor() {
    // Check if user is logged in (you would typically get this from a service)
    this.checkAuthStatus();
  }

  // Mock authentication check - replace with actual service call
  private checkAuthStatus() {
    // For demo purposes, you can set this to true to see logged-in state
    const mockUser: User = {
      id: '1',
      email: 'john.doe@example.com',
      phoneNumber: '01012345678',
      isEmailConfirmed: true,
      role: "client",
      accessToken: "132456789",
      refreshToken: "987754321"
    };

    // Set to null for logged out state, or mockUser for logged in state
    this.currentUser.set(mockUser);
    this.isLoggedIn.set(!!mockUser);
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
    return this.currentUser()?.phoneNumber || 'User';
  }

  getUserEmail(): string {
    return this.currentUser()?.email || 'user@example.com';
  }

  getUserInitials(): string {
    const name = this.getUserName();
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase();
  }

  logout() {
    // Implement your logout logic here
    this.currentUser.set(null);
    this.isLoggedIn.set(false);
    this.userMenuOpen.set(false);
    this.router.navigate(['/']);

    // In a real app, you would call your auth service:
    // this.authService.logout();
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
