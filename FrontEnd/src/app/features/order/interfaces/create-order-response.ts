import { OrderItemResponse } from './order-item-response';

export interface CreateOrderResponse {
  id: string;
  clientId: string;
  providerId: string;
  amount: number;
  commission: number;
  totalPrice: number;
  orderDate: Date;
  status: string;
  orderItem: OrderItemResponse;
  expiryDate: Date;
  downloadLink: string;
  apiKey: string;
}
