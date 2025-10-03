import { CartItemResponse } from "./cart-item-response";

export interface CartResponse {
  cartId: string;
  totalItems: number;
  totalPrice: number;
  totalCommission: number;
  items: CartItemResponse[];
}