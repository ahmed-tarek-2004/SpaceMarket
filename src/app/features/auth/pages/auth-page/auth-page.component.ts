import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SignUpPageComponent } from '../sign-up-page/sign-up-page.component';

import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { LucideAngularModule } from 'lucide-angular';
import { SignInPageComponent } from '../sign-in-page/sign-in-page.component';

@Component({
  selector: 'app-auth-page',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    SignUpPageComponent,
    MatCardModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    LucideAngularModule,
    SignInPageComponent,
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
