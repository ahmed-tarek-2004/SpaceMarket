import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { CartItemResponse } from '../../interfaces/cart-item-response';
import { CartFacadeService } from '../../services/cart-facade.service';
import { CartItemComponent } from '../../components/cart-item/cart-item.component';
import { ROUTES } from '../../../../shared/config/constants';
import { CartResponse } from '../../interfaces/cart-response';

@Component({
  selector: 'app-cart-page',
  imports: [CommonModule, CartItemComponent, RouterLink],
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.scss'],
})
export class CartPageComponent implements OnInit {
  cart: CartResponse | null = null;

  readonly ROUTES = ROUTES;

  constructor(
    private facadeCart: CartFacadeService,
    private destroyRef: DestroyRef,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.facadeCart.cartState$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((cart) => {
      this.cart = cart;
      this.cdr.detectChanges();
    });

    this.facadeCart.loadCart();
  }

  onRemoveItem(cartItemId: string): void {
    this.facadeCart.removeItem(cartItemId);
  }

  clearCart(): void {
    if (confirm('Are you sure you want to clear all items from your cart?')) {
      this.facadeCart.clearCart();
    }
  }

  get cartItems(): CartItemResponse[] {
    return this.cart?.items || [];
  }

  get totalItems(): number {
    return this.cart?.totalItems || 0;
  }

  get totalPrice(): number {
    return this.cart?.totalPrice || 0;
  }

  checkout(): void {
    console.log('Proceeding to checkout with items:', this.cartItems);
    alert(
      `Checkout not implemented yet. You have ${
        this.totalItems
      } items worth $${this.totalPrice.toFixed(2)}.`
    );
  }
}
