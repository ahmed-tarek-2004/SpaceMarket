import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, catchError, of } from 'rxjs';
import { ServiceCategory } from '../interfaces/service-category';
import { ApiResponse } from '../../../core/interfaces/api-response';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ServiceCategoryService {
  private base = `${environment.apiUrl}${environment.serviceCategory.getAllCategories}`;

  constructor(private http: HttpClient) {}

  getCategories(): Observable<ServiceCategory[]> {
    return this.http.get<ApiResponse<ServiceCategory[]>>(this.base).pipe(
      map(resp => {
        return resp?.data ?? [];
      }),
      catchError(err => {
        console.error('ServiceCategoryService.getCategories failed', err);
        return of([] as ServiceCategory[]);
      })
    );
  }
}
