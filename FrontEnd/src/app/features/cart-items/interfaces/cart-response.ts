import { CartItem } from './cart-item';

export interface CartResponse {
  cartId: string;
  cartItems: CartItem[];
  totalItems: number;
  totalPrice: number;
}
