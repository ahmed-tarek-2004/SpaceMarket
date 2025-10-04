import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { CartApiService } from './cart-api.service';
import { AddingToCartRequest } from '../interfaces/adding-to-cart-request';
import { CartItemResponse } from '../interfaces/cart-item-response';
import { CartResponse } from '../interfaces/cart-response';

@Injectable({
  providedIn: 'root',
})
export class CartFacadeService {
  private cartState = new BehaviorSubject<CartResponse | null>(null);
  cartState$ = this.cartState.asObservable();

  constructor(private api: CartApiService) {}

  loadCart(): void {
    this.api.getCartContent().subscribe({
      next: (res) => {
        console.log('Cart response from backend:', res);
        this.cartState.next(res.data);
      },
      error: (err) => console.error('Failed to load cart', err),
    });
  }

  addToCart(request: AddingToCartRequest): void {
    this.api.addToCart(request).subscribe({
      next: (res) => {
        this.cartState.next(res.data);
        console.log('Item added to cart:', res.message);
      },
      error: (err) => {
        console.error('Failed to add item to cart', err);

        if (err.error?.succeeded) {
          this.cartState.next(err.error.data);
        }
      },
    });
  }

  changeQuantity(cartItemId: string, quantity: number): void {
    this.api.updateQuantity(cartItemId, quantity).subscribe({
      next: (res) => this.cartState.next(res.data),
      error: (err) => console.error('Failed to update quantity', err),
    });
  }

  removeItem(cartItemId: string): void {
    this.api.removeItem(cartItemId).subscribe({
      next: () => {
        this.loadCart();
      },
      error: (err) => console.error('Failed to remove item', err),
    });
  }

  clearCart(): void {
    this.api.clearCart().subscribe({
      next: () => {
        this.loadCart();
      },
      error: (err) => console.error('Failed to clear cart', err),
    });
  }

  getCartItems(): CartItemResponse[] {
    return this.cartState.value?.items || [];
  }

  getTotalItems(): number {
    return this.cartState.value?.totalItems || 0;
  }

  getTotalPrice(): number {
    return this.cartState.value?.totalPrice || 0;
  }
}
