import { Component, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../services/toast.service';
import { ToastMessage } from '../../interfaces/toaster-message';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './toast-container.component.html',
  styleUrls: ['./toast-container.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToastContainerComponent implements OnDestroy {
  toast: ToastMessage | null = null;
  private sub?: Subscription;
  private timeout?: any;

  constructor(private toastService: ToastService, private cdr: ChangeDetectorRef) {
    this.sub = this.toastService.messages$.subscribe((msg) => {
      if (msg) {
        this.toast = msg;
        this.cdr.markForCheck();

        clearTimeout(this.timeout);
        this.timeout = setTimeout(() => {
          this.toast = null;
          this.cdr.markForCheck();
        }, msg.duration ?? 3000);
      }
    });
  }

  dismiss() {
    this.toast = null;
    this.cdr.markForCheck();
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
    clearTimeout(this.timeout);
  }
}
