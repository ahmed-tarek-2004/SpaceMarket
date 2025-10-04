import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { CartResponse } from '../interfaces/cart-response';
import { ApiResponse } from '../interfaces/api-response';

@Injectable({
  providedIn: 'root'
})
export class CartItemsService {
  private baseUrl = '/api/Cart';

  private cartSubject = new BehaviorSubject<CartResponse | null>(null);
  public cart$ = this.cartSubject.asObservable();

  constructor(private http: HttpClient) {}

  private updateCart(cart: CartResponse): void {
    this.cartSubject.next(cart);
  }

  addToCart(payload: { serviceId: string; dataSetId: string; quantity: number }): 
    Observable<ApiResponse<CartResponse>> {
    return this.http.post<ApiResponse<CartResponse>>(
      `${this.baseUrl}/add-to-cart`, payload
    ).pipe(
      tap(res => {
        if (res.succeeded && res.data) {
          this.updateCart(res.data);
        }
      })
    );
  }

  getCartContent(): Observable<ApiResponse<CartResponse>> {
    return this.http.get<ApiResponse<CartResponse>>(
      `${this.baseUrl}/cart-content`
    ).pipe(
      tap(res => {
        if (res.succeeded && res.data) {
          this.updateCart(res.data);
        }
      })
    );
  }

  // Update quantity
  updateQuantity(payload: { cartItemId: string; quantity: number }): 
    Observable<ApiResponse<CartResponse>> {
    return this.http.put<ApiResponse<CartResponse>>(
      `${this.baseUrl}/update-quantity`, payload
    ).pipe(
      tap(res => {
        if (res.succeeded && res.data) {
          this.updateCart(res.data);
        }
      })
    );
  }

  // Remove item
  removeItem(cartItemId: string): Observable<ApiResponse<CartResponse>> {
    return this.http.delete<ApiResponse<CartResponse>>(
      `${this.baseUrl}/remove/${cartItemId}`
    ).pipe(
      tap(res => {
        if (res.succeeded && res.data) {
          this.updateCart(res.data);
        }
      })
    );
  }

  // Clear cart
  clearCart(): Observable<ApiResponse<CartResponse>> {
    return this.http.delete<ApiResponse<CartResponse>>(
      `${this.baseUrl}/clear-cart`
    ).pipe(
      tap(res => {
        if (res.succeeded && res.data) {
          this.updateCart(res.data);
        }
      })
    );
  }
}