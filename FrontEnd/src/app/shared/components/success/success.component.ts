import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-success',
  imports: [RouterLink],
  templateUrl: './success.component.html',
  styleUrl: './success.component.scss',
})
export class SuccessComponent implements OnInit, OnDestroy {
  message = 'Everything went well.';
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
