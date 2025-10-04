import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { CartItemsService } from '../../services/cart-items.service';
import { CartItem } from '../../interfaces/cart-item';
import { CartResponse } from '../../interfaces/cart-response';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-cart-items-list',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './cart-items-list.component.html',
  styleUrls: ['./cart-items-list.component.scss']
})
export class CartItemsListComponent implements OnInit, OnDestroy {
  cart: CartResponse | null = null;
  loading = false;
  error = '';
  private subscription = new Subscription();

  constructor(private cartService: CartItemsService) {}

  ngOnInit(): void {
    this.loadCart();
    this.subscription.add(
      this.cartService.cart$.subscribe(cart => {
        this.cart = cart;
        this.loading = false;
        this.error = '';
      })
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe(); 
  }

  loadCart(): void {
    this.loading = true;
    this.error = '';
    this.cartService.getCartContent().subscribe({
      error: (err) => {
        console.error('Load cart error:', err);
        this.error = '❌ Failed to load cart';
        this.loading = false;
      }
    });
  }

  trackByCartItemId(index: number, item: CartItem): string {
    return item.cartItemId;
  }

  updateQuantity(item: CartItem, quantityStr: string): void {
    const newQuantity = Number(quantityStr);
    if (isNaN(newQuantity) || newQuantity < 1) return;

    this.cartService.updateQuantity({ cartItemId: item.cartItemId, quantity: newQuantity })
      .subscribe({
        error: (err) => {
          console.error('Update error:', err);
          this.error = '❌ Failed to update quantity';
          this.loadCart(); // fallback
        }
      });
  }

  removeItem(item: CartItem): void {
    this.cartService.removeItem(item.cartItemId).subscribe({
      error: (err) => {
        console.error('Remove error:', err);
        this.error = '❌ Failed to remove item';
        this.loadCart();
      }
    });
  }

  clearCart(): void {
    if (confirm('Are you sure you want to clear your cart?')) {
      this.cartService.clearCart().subscribe({
        error: (err) => {
          console.error('Clear error:', err);
          this.error = '❌ Failed to clear cart';
          this.loadCart();
        }
      });
    }
  }
}