import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../cart-items/interfaces/api-response';
import { DatasetResponse } from '../interfaces/dataset/dataset-response';
import { ServiceMetricsResponse } from '../interfaces/service/service-metrics-response';
import { ServiceResponse } from '../interfaces/service/service-response';

@Injectable({
  providedIn: 'root',
})
export class ProviderService {
  private baseUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  getMyServices(): Observable<ApiResponse<ServiceResponse[]>> {
    return this.http.get<ApiResponse<ServiceResponse[]>>(`${this.baseUrl}${environment.service.myServices}`);
  }

  deleteService(serviceId: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}${environment.service.deleteService}${serviceId}`);
  }

  updateService(request: any): Observable<ApiResponse<ServiceResponse>> {
    return this.http.put<ApiResponse<ServiceResponse>>(`${this.baseUrl}${environment.service.updateService}`, request);
  }

  getServiceMetrics(
    startDate?: string,
    endDate?: string
  ): Observable<ApiResponse<ServiceMetricsResponse[]>> {
    let params = new HttpParams();
    if (startDate) params = params.append('StartDate', startDate);
    if (endDate) params = params.append('EndDate', endDate);

    return this.http.get<ApiResponse<ServiceMetricsResponse[]>>(`${this.baseUrl}${environment.service.serviceMatrics}`, {
      params,
    });
  }

  getMyDatasets(): Observable<ApiResponse<DatasetResponse[]>> {
    return this.http.get<ApiResponse<DatasetResponse[]>>(`${this.baseUrl}${environment.service.myDatasets}`);
  }

  updateDataset(request: any): Observable<ApiResponse<DatasetResponse>> {
    return this.http.put<ApiResponse<DatasetResponse>>(`${this.baseUrl}${environment.service.updateDataset}`, request);
  }

  deleteDataset(datasetId: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}${environment.service.deleteDataset}${datasetId}`);
  }
}
