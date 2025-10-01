import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CartApiService {
  private baseUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  getCartContent(): Observable<any> {
    return this.http.get(`${this.baseUrl}${environment.cart.cartContent}`);
  }

  updateQuantity(cartItemId: string, quantity: number): Observable<any> {
    return this.http.put(`${this.baseUrl}${environment.cart.updateQuantity}`, { cartItemId, quantity });
  }

  removeItem(cartItemId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}${environment.cart.removeItem}/${cartItemId}`);
  }

  clearCart(): Observable<any> {
    return this.http.delete(`${this.baseUrl}${environment.cart.clearCart}`);
  }
}
