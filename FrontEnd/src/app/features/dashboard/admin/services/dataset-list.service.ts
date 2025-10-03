import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { DatasetListFilterRequest } from '../interfaces/dataset/dataset-list-filter-request';
import { UpdateDatasetStatusRequest } from '../interfaces/dataset/update-dataset-status-request';

@Injectable({
  providedIn: 'root',
})
export class DatasetListService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getDatasets(filters: DatasetListFilterRequest): Observable<any> {
    let params = new HttpParams();
    if (filters.categoryId) params = params.set('categoryId', filters.categoryId);
    if (filters.providerId) params = params.set('providerId', filters.providerId);
    if (filters.status) params = params.set('status', filters.status);
    if (filters.minPrice != null) params = params.set('minPrice', filters.minPrice.toString());
    if (filters.maxPrice != null) params = params.set('maxPrice', filters.maxPrice.toString());
    if (filters.pageNumber != null)
      params = params.set('pageNumber', filters.pageNumber.toString());
    if (filters.pageSize != null) params = params.set('pageSize', filters.pageSize.toString());

    const url = `${this.baseUrl}${environment.service.adminDatasetList}`;

    return this.http.get<any>(url, { params });
  }

  updateDatasetStatus(request: UpdateDatasetStatusRequest): Observable<any> {
    const url = `${this.baseUrl}${environment.service.adminUpdateDatasetStatus}`;
    return this.http.put<any>(url, request);
  }
}
