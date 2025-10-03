import { Inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { ToastService } from '../../shared/services/toast.service';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, of, catchError } from 'rxjs';
import { ApiResponse } from '../../core/interfaces/api-response';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../../environments/environment';
import { INotificationResponse } from '../interfaces/notification';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private hubConnection!: signalR.HubConnection;
  private isBrowser: boolean;
  private currentUserId: string | null = null;

  // Notification state management
  private _notifications = signal<INotificationResponse[]>([]);
  private _unreadCount = signal(0);
  private _isConnected = signal(false);

  // Public readonly signals for components
  readonly notifications = this._notifications.asReadonly();
  readonly unreadCount = this._unreadCount.asReadonly();
  readonly isConnected = this._isConnected.asReadonly();

  // BehaviorSubject for real-time updates
  private notificationsSubject = new BehaviorSubject<INotificationResponse[]>([]);
  public notifications$ = this.notificationsSubject.asObservable();

  constructor(
    private toast: ToastService,
    private http: HttpClient,
    private tokenService: TokenService,
    @Inject(PLATFORM_ID) private platformId: object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  startConnection(): void {
    if (!this.isBrowser) return;

    const accessToken = this.tokenService.getAccessToken();
    if (!accessToken) return;

    // Get current user ID
    this.currentUserId = this.getCurrentUserId();
    if (!this.currentUserId) return;

    const hubUrl = 'https://spacemarket.runasp.net/hub/notifications';
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => accessToken,
      })
      .withAutomaticReconnect([0, 2000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Add connection state handling
    this.hubConnection.onreconnecting(() => {
      console.log('🔄 Reconnecting to notifications...');
      this._isConnected.set(false);
    });

    this.hubConnection.onreconnected(() => {
      console.log('✅ Reconnected to notifications');
      this._isConnected.set(true);
    });

    // Start connection
    this.hubConnection
      .start()
      .then(() => {
        console.log('✅ SignalR connected');
        this._isConnected.set(true);
        this.loadNotifications();
      })
      .catch((err) => {
        console.error('❌ SignalR connection error:', err);
        this._isConnected.set(false);
        this.toast.error('Failed to connect to notifications', 5000);
      });

    // Handle incoming notifications
    this.hubConnection.on('ReceiveNotification', (notification: INotificationResponse) => {
      this.handleIncomingNotification(notification);
    });
  }

  private getCurrentUserId(): string | null {
    const storedUser = this.tokenService.getUserId();

    return storedUser ?? null;
  }

  private handleIncomingNotification(notification: INotificationResponse): void {
    if (!this.isBrowser || !notification) return;

    // Check if notification is for current user
    if (notification.recipientId !== this.currentUserId) return;

    // Add to notifications list
    this.addNotification(notification);

    // Show toast notification
    this.toast.info(notification.message, 6000);

    // Log title details
    try {
      const title = notification.title ? JSON.parse(notification.title) : null;
      console.log('🧾 Notification details:', title);
    } catch (e) {
      console.warn('⚠️ Failed to read title:', e);
    }
  }

  private addNotification(notification: INotificationResponse): void {
    const currentNotifications = this._notifications();
    const updatedNotifications = [notification, ...currentNotifications];

    this._notifications.set(updatedNotifications);
    this.notificationsSubject.next(updatedNotifications);

    // Update unread count
    if (!notification.isRead) {
      this._unreadCount.update((count) => count + 1);
    }
  }

  private loadNotifications(): void {
    if (!this.currentUserId) return;

    this.getNotifications().subscribe({
      next: (response: ApiResponse<INotificationResponse[]>) => {
        if (response.succeeded && response.data) {
          const userNotifications = response.data.filter(
            (n) => n.recipientId === this.currentUserId
          );
          this._notifications.set(userNotifications);
          this.notificationsSubject.next(userNotifications);

          // Calculate unread count
          const unreadCount = userNotifications.filter((n) => !n.isRead).length;
          this._unreadCount.set(unreadCount);
        }
      },
      error: (error) => {
        console.error('Failed to load notifications:', error);
        this.toast.error('Failed to load notifications', 5000);
      },
    });
  }

  getNotifications(): Observable<ApiResponse<INotificationResponse[]>> {
    return this.http.get<ApiResponse<INotificationResponse[]>>(
      `${environment.apiUrl}${environment.notification.getAll}`
    );
  }

  getNotificationsForAdmin(): Observable<ApiResponse<INotificationResponse[]>> {
    return this.http.get<ApiResponse<INotificationResponse[]>>(
      `${environment.apiUrl}${environment.notification.getAllForAdmin}`
    );
  }

  markNotificationAsRead(id: string): Observable<ApiResponse<INotificationResponse>> {
    return this.http.post<ApiResponse<INotificationResponse>>(
      `${environment.apiUrl}${environment.notification.markAsRead(id)}`,
      {}
    );
  }

  markAsRead(id: string): void {
    const notifications = this._notifications();
    const notificationIndex = notifications.findIndex((n) => n.id === id);

    if (notificationIndex !== -1 && !notifications[notificationIndex].isRead) {
      // Update local state immediately for better UX
      const updatedNotifications = [...notifications];
      updatedNotifications[notificationIndex] = {
        ...updatedNotifications[notificationIndex],
        isRead: true,
      };

      this._notifications.set(updatedNotifications);
      this.notificationsSubject.next(updatedNotifications);

      // Update unread count
      this._unreadCount.update((count) => Math.max(0, count - 1));

      // Call API to mark as read on server
      this.markNotificationAsRead(id).subscribe({
        error: (error) => {
          console.error('Failed to mark notification as read:', error);
          // Revert local state on error
          const revertedNotifications = [...notifications];
          this._notifications.set(revertedNotifications);
          this.notificationsSubject.next(revertedNotifications);
          this._unreadCount.update((count) => count + 1);
        },
      });
    }
  }

  markAllAsRead(): void {
    const notifications = this._notifications();
    const unreadNotifications = notifications.filter((n) => !n.isRead);

    if (unreadNotifications.length === 0) return;

    // Update local state
    const updatedNotifications = notifications.map((n) => ({ ...n, isRead: true }));
    this._notifications.set(updatedNotifications);
    this.notificationsSubject.next(updatedNotifications);
    this._unreadCount.set(0);

    // Mark each notification as read on server
    unreadNotifications.forEach((notification) => {
      this.markNotificationAsRead(notification.id).subscribe({
        error: (error) => {
          console.error(`Failed to mark notification ${notification.id} as read:`, error);
        },
      });
    });
  }

  clearNotifications(): void {
    this._notifications.set([]);
    this.notificationsSubject.next([]);
    this._unreadCount.set(0);
  }

  refreshNotifications(): void {
    this.loadNotifications();
  }

  stopConnection(): void {
    if (this.isBrowser && this.hubConnection) {
      this.hubConnection.stop();
      this._isConnected.set(false);
    }
  }
}
