export interface CheckoutRequest {
  serviceName: string;
  serviceUnitAmount: number;
  orderId: string;
  currency: string;
  successUrl: string;
  cancelUrl: string;
}
