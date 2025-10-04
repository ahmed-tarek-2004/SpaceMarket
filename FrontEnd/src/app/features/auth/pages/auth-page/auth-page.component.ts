import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SignUpPageComponent } from '../sign-up-page/sign-up-page.component';
import { SignInPageComponent } from '../sign-in-page/sign-in-page.component';
import { MatTab, MatTabGroup } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { Star } from '../../interfaces/star';
import { filter } from 'rxjs/operators';

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
export class AuthPageComponent implements OnInit {
  Math = Math;
  selectedTabIndex = 0;
  stars: Star[] = [];

  constructor(private route: ActivatedRoute, private router: Router) {
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

  ngOnInit() {
    // Listen to route changes to update tab index
    this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
      this.updateTabFromRoute();
    });

    // Initial tab setup
    this.updateTabFromRoute();
  }

  private updateTabFromRoute() {
    const currentUrl = this.router.url;

    if (currentUrl.includes('/auth/register') || currentUrl.includes('/sign-up')) {
      this.selectedTabIndex = 1;
    } else {
      this.selectedTabIndex = 0;
    }
  }

  onTabChange(index: number) {
    this.selectedTabIndex = index;

    // Navigate to the corresponding route
    if (index === 0) {
      this.router.navigate(['/auth/sign-in']);
    } else {
      this.router.navigate(['/auth/register']);
    }
  }
}
