import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartItemsFormComponent } from '../../components/cart-items-form/cart-items-form.component';
import { CartItemsListComponent } from '../../components/cart-items-list/cart-items-list.component';

@Component({
  selector: 'app-cart-items-page',
  standalone: true,
  imports: [CommonModule, CartItemsFormComponent, CartItemsListComponent],
  templateUrl: './cart-items-page.component.html',
  styleUrls: ['./cart-items-page.component.scss']
})
export class CartItemsPageComponent {}
