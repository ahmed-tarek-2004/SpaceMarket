import { Component, signal, computed, OnDestroy, inject, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../../core/services/notification.service';
import { INotificationResponse } from '../../../core/interfaces/notification';

@Component({
  selector: 'app-notification-bell',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-bell.component.html',
  styleUrls: ['./notification-bell.component.scss'],
})
export class NotificationBellComponent implements OnDestroy {
  private notificationService = inject(NotificationService);

  // Component state
  isDropdownOpen = signal(false);

  // Notification data from service
  notifications = this.notificationService.notifications;
  unreadCount = this.notificationService.unreadCount;
  isConnected = this.notificationService.isConnected;

  // Computed values
  hasUnread = computed(() => this.unreadCount() > 0);
  displayCount = computed(() => {
    const count = this.unreadCount();
    return count > 99 ? '99+' : count.toString();
  });

  toggleDropdown() {
    this.isDropdownOpen.update((open) => !open);

    // Mark all as read when opening dropdown
    if (!this.isDropdownOpen()) {
      this.notificationService.markAllAsRead();
    }
  }

  markAsRead(notification: INotificationResponse) {
    this.notificationService.markAsRead(notification.id);
  }

  markAllAsRead() {
    this.notificationService.markAllAsRead();
  }

  refreshNotifications() {
    this.notificationService.refreshNotifications();
  }

  formatNotificationTime(createdAt: string): string {
    const date = new Date(createdAt);
    const now = new Date();
    const diffInMinutes = Math.floor((now.getTime() - date.getTime()) / (1000 * 60));

    if (diffInMinutes < 1) return 'Just now';
    if (diffInMinutes < 60) return `${diffInMinutes}m ago`;
    if (diffInMinutes < 1440) return `${Math.floor(diffInMinutes / 60)}h ago`;
    return `${Math.floor(diffInMinutes / 1440)}d ago`;
  }

  // Close dropdown when clicking outside
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event) {
    const target = event.target as HTMLElement;
    if (!target.closest('.notification-bell-container')) {
      this.isDropdownOpen.set(false);
    }
  }

  // Close dropdown on escape key
  @HostListener('window:keydown.escape')
  onEscape() {
    this.isDropdownOpen.set(false);
  }

  ngOnDestroy() {
    // Component cleanup if needed
  }
}
