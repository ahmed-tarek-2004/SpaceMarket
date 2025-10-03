import { Component, inject, input } from '@angular/core';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { PaymentServiceApi } from '../../services/payment-service-api';
import { CheckoutRequest } from '../../interfaces/checkout-request';
import { Router } from '@angular/router';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-payment-button',
  imports: [ButtonComponent],
  templateUrl: './payment-button.html',
  styleUrl: './payment-button.scss',
})
export class PaymentButton {
  isLoading = false;
  itemData = input<CheckoutRequest>();
  private paymentApi = inject(PaymentServiceApi);
  private router = inject(Router);
  private toast = inject(ToastService);

  initiatePayment() {
    this.isLoading = true;
    this.paymentApi.checkout(this.itemData() as CheckoutRequest).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.router.navigate([res.data.sessionUrl, { sessionId: res.data.sessionId }]);
      },
      error: (err) => {
        this.isLoading = false;
        this.toast.error(err.error.message);
      },
    });
  }
}
