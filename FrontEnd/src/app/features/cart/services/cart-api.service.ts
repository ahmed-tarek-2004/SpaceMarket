import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/interfaces/api-response';
import { AddingToCartRequest } from '../interfaces/adding-to-cart-request';
import { CartResponse } from '../interfaces/cart-response';

@Injectable({
  providedIn: 'root',
})
export class CartApiService {
  private baseUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  getCartContent(): Observable<ApiResponse<CartResponse>> {
    return this.http.get<ApiResponse<CartResponse>>(
      `${this.baseUrl}${environment.cart.cartContent}`
    );
  }

  addToCart(request: AddingToCartRequest): Observable<ApiResponse<CartResponse>> {
    return this.http.post<ApiResponse<CartResponse>>(
      `${this.baseUrl}${environment.cart.addToCart}`,
      request
    );
  }

  updateQuantity(cartItemId: string, quantity: number): Observable<ApiResponse<CartResponse>> {
    return this.http.put<ApiResponse<CartResponse>>(
      `${this.baseUrl}${environment.cart.updateQuantity}`,
      { cartItemId, quantity }
    );
  }

  removeItem(cartItemId: string): Observable<ApiResponse<CartResponse>> {
    return this.http.delete<ApiResponse<CartResponse>>(
      `${this.baseUrl}${environment.cart.removeItem}${cartItemId}`
    );
  }

  clearCart(): Observable<ApiResponse<CartResponse>> {
    return this.http.delete<ApiResponse<CartResponse>>(
      `${this.baseUrl}${environment.cart.clearCart}`
    );
  }
}
