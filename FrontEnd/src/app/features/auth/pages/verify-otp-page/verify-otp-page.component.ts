import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VerifyOtpFormComponent } from '../../components/verify-otp-form/verify-otp-form.component';
import { Star } from '../../interfaces/star';
import { RouterLink } from '@angular/router';
import { ROUTES } from '../../../../shared/config/constants';

@Component({
  selector: 'app-verify-otp-page',
  standalone: true,
  imports: [CommonModule, VerifyOtpFormComponent, RouterLink],
  templateUrl: './verify-otp-page.component.html',
  styleUrls: ['./verify-otp-page.component.scss'],
})
export class VerifyOtpPageComponent implements OnInit {
  stars: Star[] = [];
  readonly ROUTES = ROUTES

  ngOnInit() {
    this.generateStars(30);
  }

  private generateStars(count: number) {
    for (let i = 0; i < count; i++) {
      this.stars.push({
        left: Math.random() * 100,
        top: Math.random() * 100,
        delay: Math.random() * 3,
      });
    }
  }
}
