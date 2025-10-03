import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ServiceDetailsResponse } from '../interfaces/service.interface';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/interfaces/api-response';

@Injectable({
  providedIn: 'root',
})
export class ServiceDetailService {
  private baseUrl = `${environment.apiUrl}${environment.service.serviceDetail}`;

  constructor(private http: HttpClient) {}

  getService(serviceId: string): Observable<ServiceDetailsResponse> {
    return this.http
      .get<ApiResponse<ServiceDetailsResponse>>(`${this.baseUrl}${serviceId}`)
      .pipe(map((response) => response.data));
  }
}
