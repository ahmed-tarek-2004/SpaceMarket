import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ForgetPasswordFormComponent } from '../../components/sign-in/forget-password-form/forget-password-form.component';
import { Star } from '../../interfaces/star';
import { ROUTES } from '../../../../shared/config/constants';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-forget-password-page',
  standalone: true,
  imports: [CommonModule, ForgetPasswordFormComponent, RouterLink],
  templateUrl: './forget-password-page.component.html',
  styleUrls: ['./forget-password-page.component.scss'],
})
export class ForgetPasswordPageComponent {
  stars: Star[] = [];
  readonly ROUTES = ROUTES;

  constructor() {
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
