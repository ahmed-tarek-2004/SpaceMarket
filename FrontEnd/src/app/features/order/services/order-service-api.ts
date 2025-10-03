import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { CreateOrderRequest } from '../interfaces/create-order-request';
import { Observable } from 'rxjs';
import { CreateOrderResponse } from '../interfaces/create-order-response';
import { ApiResponse } from '../../../core/interfaces/api-response';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class OrderServiceApi {
  private readonly _apiUrl = `${environment.apiUrl}${environment.order.createOrder}`;

  constructor(private http: HttpClient) {}

  createOrder(order: CreateOrderRequest): Observable<ApiResponse<CreateOrderResponse>> {
    return this.http.post<ApiResponse<CreateOrderResponse>>(this._apiUrl, order);
  }
}
