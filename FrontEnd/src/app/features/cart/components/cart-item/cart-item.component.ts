import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartItemResponse } from '../../interfaces/cart-item-response';
import { CartFacadeService } from '../../services/cart-facade.service';

@Component({
  selector: 'app-cart-item',
  imports: [CommonModule],
  templateUrl: './cart-item.component.html',
  styleUrls: ['./cart-item.component.scss'],
})
export class CartItemComponent {
  @Input() cartItem!: CartItemResponse;
  private cartFacade = inject(CartFacadeService);

  getProviderInitial(providerName?: string): string {
    return providerName?.charAt(0).toUpperCase() || 'P';
  }

  removeItem() {
    this.cartFacade.removeItem(this.cartItem.cartItemId);
  }
}
