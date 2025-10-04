import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceListFilterRequest } from '../interfaces/service/service-list-filter-request';
import { UpdateServiceStatusRequest } from '../interfaces/service/update-service-status-request';
import { ServiceResponse } from '../interfaces/service/service-response';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../components/service-category/interfaces/api-response.interface';

@Injectable({
  providedIn: 'root',
})
export class ServiceListService {
  private baseUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  getServices(filter: ServiceListFilterRequest): Observable<ApiResponse<ServiceResponse[]>> {
    let params = new HttpParams();

    if (filter.categoryId) {
      params = params.append('CategoryId', filter.categoryId);
    }
    if (filter.providerId) {
      params = params.append('ProviderId', filter.providerId);
    }
    if (filter.status) {
      params = params.append('Status', filter.status);
    }

    return this.http.get<ApiResponse<ServiceResponse[]>>(
      `${this.baseUrl}${environment.service.adminServiceList}`,
      { params }
    );
  }

  updateServiceStatus(request: UpdateServiceStatusRequest): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(
      `${this.baseUrl}${environment.service.adminUpdateServiceStatus}`,
      request
    );
  }
}
