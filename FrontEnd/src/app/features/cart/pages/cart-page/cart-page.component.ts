import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { CartItem } from '../../interfaces/cart-item';
import { CartFacadeService } from '../../services/cart-facade.service';
import { CartItemComponent } from '../../components/cart-item/cart-item.component';
import { ROUTES } from '../../../../shared/config/constants';

@Component({
  selector: 'app-cart-page',
  imports: [CommonModule, CartItemComponent, RouterLink],
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.scss'],
})
export class CartPageComponent implements OnInit {
  cartItems: CartItem[] = [];

  readonly ROUTES = ROUTES;

  private destroyRef = inject(DestroyRef);

  constructor(private facadeCart: CartFacadeService) {}

  ngOnInit(): void {
    this.facadeCart.cartState$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((items) => {
      this.cartItems = items ?? [];
    });

    this.facadeCart.loadCart();
  }

  onQuantityChange({ cartItemId, quantity }: { cartItemId: string; quantity: number }): void {
    this.facadeCart.changeQuantity(cartItemId, quantity);
  }

  onRemoveItem(cartItemId: string): void {
    this.facadeCart.removeItem(cartItemId);
  }

  clearCart(): void {
    if (confirm('Are you sure you want to clear all items from your cart?')) {
      this.facadeCart.clearCart();
    }
  }

  get totalItems(): number {
    return this.cartItems.reduce((acc, item) => acc + item.quantity, 0);
  }
  
  get totalPrice(): number {
    return this.cartItems.reduce((acc, item) => acc + item.total, 0);
  }

  checkout(): void {
    console.log('Proceeding to checkout with items:', this.cartItems);
    alert(
      `Checkout not implemented yet. You have ${
        this.totalItems
      } services worth $${this.totalPrice.toFixed(2)}.`
    );
  }
}
