import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ButtonComponent } from '../button/button.component';

@Component({
  selector: 'app-error',
  imports: [RouterLink],
  template: `
    <div class="text-center mt-20">
      <h1 class="text-3xl font-bold text-red-600">❌ Error</h1>
      <p class="mt-4 text-gray-600">
        {{ message }}
      </p>
      <button-component routerLink="/Home" class="m-4">Go to Homepage</button-component>
    </div>
  `,
  styleUrl: './error.component.css',
})
export class ErrorComponent {
  message = 'An error occurred.';

  countdown = 5;
  private countdownIntervalId: any;
  private redirectTimeoutId: any;

  constructor(private route: ActivatedRoute, private router: Router) {
    this.route.queryParams.subscribe((params) => {
      if (params['message']) {
        this.message = params['message'];
      }
    });
  }

  ngOnInit(): void {
    this.countdownIntervalId = setInterval(() => {
      if (this.countdown > 0) {
        this.countdown -= 1;
      }
    }, 1000);

    this.redirectTimeoutId = setTimeout(() => {
      this.router.navigate(['/home']);
    }, 5000);
  }

  ngOnDestroy(): void {
    if (this.countdownIntervalId) {
      clearInterval(this.countdownIntervalId);
    }
    if (this.redirectTimeoutId) {
      clearTimeout(this.redirectTimeoutId);
    }
  }
}
