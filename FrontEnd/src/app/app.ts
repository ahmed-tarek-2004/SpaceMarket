import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { ToastContainerComponent } from './shared/components/toast-container/toast-container.component';
import { HeaderComponent } from './shared/components/header/header.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { Subscription, filter } from 'rxjs';
import { AuthService } from './core/services/auth.service';
import { TokenService } from './core/services/token.service';
import { ApiResponse } from './core/interfaces/api-response';
import { INotificationResponse } from './core/interfaces/notification';
import { NotificationService } from './core/services/notification.service';

@Component({
  selector: 'app-root',
  imports: [ToastContainerComponent, RouterOutlet, CommonModule, HeaderComponent, FooterComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit, OnDestroy {
  showHeader = signal<boolean>(true);
  showFooter = signal<boolean>(true);

  private sub = new Subscription();

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private tokenService: TokenService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    const userId = this.tokenService.getUserId();

    if (userId) {
      // Start notification connection - service handles state management
      this.notificationService.startConnection();
    }

    this.sub.add(
      this.router.events.pipe(filter((e) => e instanceof NavigationEnd)).subscribe(() => {
        let snapshot = this.activatedRoute.snapshot;
        while (snapshot.firstChild) {
          snapshot = snapshot.firstChild;
        }

        const hideHeader = !!snapshot.data?.['hideHeader'];
        const hideFooter = !!snapshot.data?.['hideFooter'];

        this.showHeader.set(!hideHeader);
        this.showFooter.set(!hideFooter);
      })
    );

    // Initial check
    let deepest = this.activatedRoute.snapshot;
    while (deepest.firstChild) {
      deepest = deepest.firstChild;
    }
    this.showHeader.set(!Boolean(deepest.data?.['hideHeader']));
    this.showFooter.set(!Boolean(deepest.data?.['hideFooter']));
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
