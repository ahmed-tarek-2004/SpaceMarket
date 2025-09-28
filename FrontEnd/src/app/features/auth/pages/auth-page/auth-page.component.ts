import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SignUpPageComponent } from '../sign-up-page/sign-up-page.component';
import { SignInPageComponent } from '../sign-in-page/sign-in-page.component';
import { MatTab, MatTabGroup } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { Star } from '../../interfaces/star';

@Component({
  selector: 'app-auth-page',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTabGroup,
    MatTab,
    RouterModule,
    SignUpPageComponent,
    SignInPageComponent,
    MatCardModule,
  ],
  templateUrl: './auth-page.component.html',
  styleUrls: ['./auth-page.component.scss'],
})
export class AuthPageComponent {
  Math = Math;

  selectedTabIndex = 0;
  selectedTab: 'login' | 'register' = 'login';

  stars: Star[] = [];

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

  onTabChange(index: number) {
    this.selectedTabIndex = index;
    this.selectedTab = index === 0 ? 'login' : 'register';
  }
}
