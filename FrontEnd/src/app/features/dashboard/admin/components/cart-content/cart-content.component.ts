import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-cart-content',
  imports: [FormsModule, CommonModule],
  templateUrl: './cart-content.component.html',
  styleUrls: ['./cart-content.component.scss'],
})
export class CartContentComponent {
  searchClientId = '';
  loading = false;
  cartData: any = null;

  async fetchCartContent() {
    if (!this.searchClientId) return;

    this.loading = true;
    try {
      // Simulate API call
      const response = await fetch(`/api/Cart/Admin/cart-content?clientId=${this.searchClientId}`);
      this.cartData = await response.json();
    } catch (error) {
      console.error('Error fetching cart content:', error);
    } finally {
      this.loading = false;
    }
  }
}
