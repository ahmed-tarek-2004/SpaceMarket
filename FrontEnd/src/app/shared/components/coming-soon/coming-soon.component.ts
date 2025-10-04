import { Component, OnDestroy, OnInit, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-coming-soon',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './coming-soon.component.html',
  styleUrls: ['./coming-soon.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ComingSoonComponent implements OnInit, OnDestroy {
  targetDate = new Date('2025-12-01T00:00:00');
  countdown = { days: '00', hours: '00', minutes: '00', seconds: '00' };
  private intervalId: any;

  constructor(private cdRef: ChangeDetectorRef) {}

  ngOnInit() {
    this.startCountdown();
  }

  private startCountdown() {
    this.intervalId = setInterval(() => {
      const now = new Date().getTime();
      const distance = this.targetDate.getTime() - now;

      if (distance < 0) {
        this.updateCountdown('00', '00', '00', '00');
        clearInterval(this.intervalId);
        return;
      }

      const days = Math.floor(distance / (1000 * 60 * 60 * 24));
      const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
      const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
      const seconds = Math.floor((distance % (1000 * 60)) / 1000);

      this.updateCountdown(
        days.toString().padStart(2, '0'),
        hours.toString().padStart(2, '0'),
        minutes.toString().padStart(2, '0'),
        seconds.toString().padStart(2, '0')
      );
    }, 1000);
  }

  private updateCountdown(days: string, hours: string, minutes: string, seconds: string) {
    this.countdown = { days, hours, minutes, seconds };

    this.cdRef.detectChanges();
  }

  ngOnDestroy() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }
}
