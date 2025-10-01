import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartItem } from '../../interfaces/cart-item';
import { CartFacadeService } from '../../services/cart-facade.service';

@Component({
  selector: 'app-cart-item',
  imports: [CommonModule],
  templateUrl: './cart-item.component.html',
  styleUrls: ['./cart-item.component.scss'],
})
export class CartItemComponent {
  @Input() cartItem!: CartItem;
  private cartFacade = inject(CartFacadeService);

  updateQuantity(newQuantity: number) {
    if (newQuantity <= 0) {
      this.removeItem();
    } else {
      this.cartFacade.changeQuantity(this.cartItem.cartItemId, newQuantity);
    }
  }

  removeItem() {
    this.cartFacade.removeItem(this.cartItem.cartItemId);
  }
}
