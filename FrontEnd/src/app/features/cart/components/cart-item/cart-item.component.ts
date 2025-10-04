import { Component, Input, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartItemResponse } from '../../interfaces/cart-item-response';
import { CartFacadeService } from '../../services/cart-facade.service';
import { CheckoutRequest } from '../../../payment/interfaces/checkout-request';
import { OrderServiceApi } from '../../../order/services/order-service-api';
import { CreateOrderRequest } from '../../../order/interfaces/create-order-request';
import { TokenService } from '../../../../core/services/token.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { CreateOrderResponse } from '../../../order/interfaces/create-order-response';
import { PaymentServiceApi } from '../../../payment/services/payment-service-api';
import { Router } from '@angular/router';
import { URLS } from '../../../../shared/config/constants';

@Component({
  selector: 'app-cart-item',
  imports: [CommonModule],
  templateUrl: './cart-item.component.html',
  styleUrls: ['./cart-item.component.scss'],
})
export class CartItemComponent implements OnInit {
  @Input() cartItem!: CartItemResponse;
  checkoutRequest: CheckoutRequest = {} as CheckoutRequest;
  createOrderRequest: CreateOrderRequest = {} as CreateOrderRequest;
  currentUserId: string | null = null;
  currency = 'USD';

  isLoading = false;

  private tokenService = inject(TokenService);
  private cartFacade = inject(CartFacadeService);
  private orderService = inject(OrderServiceApi);
  private paymentService = inject(PaymentServiceApi);
  private router = inject(Router);
  private toast = inject(ToastService);

  ngOnInit(): void {
    this.currentUserId = this.tokenService.getUserId();
  }

  successUrl = URLS.success;
  cancelUrl = URLS.cancel;

  getProviderInitial(providerName?: string): string {
    return providerName?.charAt(0).toUpperCase() || 'P';
  }

  removeItem() {
    this.cartFacade.removeItem(this.cartItem.cartItemId);
  }

  createOrder() {
    this.createOrderRequest = {
      clientId: this.currentUserId,
      orderItem: {
        itemId: this.cartItem.itemId,
        type: this.cartItem.itemType,
        priceSnapshot: this.cartItem.unitPrice,
      },
    };
    this.orderService.createOrder(this.createOrderRequest).subscribe({
      next: (response) => {
        this.toast.success('Order created successfully');
        this.initiatePayment(response.data);
      },
      error: (error) => {
        console.log(error);
      },
      complete: () => {
        console.log('Order created');
      },
    });
  }

  initiatePayment(order: CreateOrderResponse) {
    this.checkoutRequest = {
      serviceName: this.cartItem.title,
      serviceUnitAmount: order.totalPrice,
      orderId: order.id,
      currency: this.currency,
      successUrl: this.successUrl,
      cancelUrl: this.cancelUrl,
    };
    this.isLoading = true;
    this.paymentService.checkout(this.checkoutRequest as CheckoutRequest).subscribe({
      next: (res) => {
        this.isLoading = false;
        window.location.href = res.data.sessionUrl;
      },
      error: (err) => {
        this.isLoading = false;
        this.toast.error(err.error.message);
      },
    });
  }
}
