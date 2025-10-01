import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { CartApiService } from './cart-api.service';
import { CartItem } from '../interfaces/cart-item';

@Injectable({
  providedIn: 'root',
})
export class CartFacadeService {
  private cartState = new BehaviorSubject<CartItem[]>([]);
  cartState$ = this.cartState.asObservable();

  constructor(private api: CartApiService) {}

  loadCart(): void {
    this.api.getCartContent().subscribe({
      next: (res) => this.cartState.next(res.data),
      error: (err) => console.error('Failed to load cart', err),
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
      next: (res) => this.cartState.next(res.data),
      error: (err) => console.error('Failed to remove item', err),
    });
  }

  clearCart(): void {
    this.api.clearCart().subscribe({
      next: (res) => this.cartState.next(res.data),
      error: (err) => console.error('Failed to clear cart', err),
    });
  }
}
