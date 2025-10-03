import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ApiResponse } from '../../../core/interfaces/api-response';
import { PagedItems } from '../../../core/interfaces/paged-items';
import { ApiServiceItem } from '../interfaces/api-service-item';
import { ServiceQuery } from '../interfaces/service-query';
import { environment } from '../../../../environments/environment';
import { ApiDatasetItem } from '../interfaces/api-dataset-item';

@Injectable({
  providedIn: 'root',
})
export class MarketplaceApiService {
  private base = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  getAvailableServices(query?: ServiceQuery): Observable<PagedItems<ApiServiceItem>> {
    let params = new HttpParams();
    if (query) {
      if (query.CategoryId) params = params.set('CategoryId', query.CategoryId);
      if (query.MinPrice != null) params = params.set('MinPrice', String(query.MinPrice));
      if (query.MaxPrice != null) params = params.set('MaxPrice', String(query.MaxPrice));
      if (query.Location) params = params.set('Location', query.Location);
      if (query.PageNumber != null) params = params.set('PageNumber', String(query.PageNumber));
      if (query.PageSize != null) params = params.set('PageSize', String(query.PageSize));
    }

    return this.http.get<ApiResponse<PagedItems<ApiServiceItem>>>(`${this.base}${environment.service.availableService}`, { params }).pipe(
      map(
        (response) =>
          response.data ??
          ({
            items: [],
            pageNumber: 1,
            totalPages: 1,
            hasPreviousPage: false,
            hasNextPage: false,
          } as PagedItems<ApiServiceItem>)
      ),
      catchError((err) => {
        console.error('MarketplaceApiService.getAvailableServices failed', err);
        return of({
          items: [],
          pageNumber: 1,
          totalPages: 1,
          hasPreviousPage: false,
          hasNextPage: false,
        } as PagedItems<ApiServiceItem>);
      })
    );
  }

  // ===================================

  getAvailableDatasets(query?: ServiceQuery): Observable<PagedItems<ApiDatasetItem>> {
    let params = new HttpParams();
    if (query) {
      if (query.CategoryId) params = params.set('CategoryId', query.CategoryId);
      if (query.MinPrice != null) params = params.set('MinPrice', String(query.MinPrice));
      if (query.MaxPrice != null) params = params.set('MaxPrice', String(query.MaxPrice));
      if (query.Location) params = params.set('Location', query.Location);
      if (query.PageNumber != null) params = params.set('PageNumber', String(query.PageNumber));
      if (query.PageSize != null) params = params.set('PageSize', String(query.PageSize));
    }


    return this.http.get<ApiResponse<PagedItems<ApiDatasetItem>>>(`${this.base}${environment.service.availableDataset}`, { params }).pipe(
      map(
        (response) =>
          response.data ?? {
            items: [],
            pageNumber: 1,
            totalPages: 1,
            hasPreviousPage: false,
            hasNextPage: false,
          }
      ),
      catchError((err) => {
        console.error('MarketplaceApiService.getAvailableDatasets failed', err);
        return of({
          items: [],
          pageNumber: 1,
          totalPages: 1,
          hasPreviousPage: false,
          hasNextPage: false,
        } as PagedItems<ApiDatasetItem>);
      })
    );
  }
}
