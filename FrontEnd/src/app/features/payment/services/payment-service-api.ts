import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../cart-items/interfaces/api-response';
import { Observable } from 'rxjs';
import { CheckoutRequest } from '../interfaces/checkout-request';
import { CheckoutResponse } from '../interfaces/checkout-response';

@Injectable({
  providedIn: 'root',
})
export class PaymentServiceApi {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}`;

  checkout(checkoutRequest: CheckoutRequest): Observable<ApiResponse<CheckoutResponse>> {
    return this.http.post<ApiResponse<CheckoutResponse>>(
      `${this.apiUrl}${environment.payment.checkoutSession}`,
      checkoutRequest
    );
  }
}
