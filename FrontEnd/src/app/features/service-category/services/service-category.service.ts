import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ServiceCategory } from '../interfaces/service-category';
import { ApiResponse } from '../../../core/interfaces/api-response';

@Injectable({ providedIn: 'root' })
export class ServiceCategoryService {
  private baseUrl = 'https://spacemarket.runasp.net/api/ServiceCategory'; 

  constructor(private http: HttpClient) {}
  
  private getHeaders() {
  const fakeToken = 'dummy-token-for-testing';
  return {
    headers: {
      'Authorization': `Bearer ${fakeToken}`,
      'Content-Type': 'application/json'
    }
  };
}

  getCategories(): Observable<ApiResponse<ServiceCategory[]>> {
    return this.http.get<ApiResponse<ServiceCategory[]>>(this.baseUrl);
  }

  getCategoryById(id: string): Observable<ServiceCategory> {
    return this.http.get<ApiResponse<ServiceCategory>>(`${this.baseUrl}/${id}`).pipe(
      map(res => res.data),
      catchError(this.handleError)
    );
  }

  createCategory(category: Omit<ServiceCategory, 'id' | 'createdAt'>): Observable<ServiceCategory> {
    return this.http.post<ApiResponse<ServiceCategory>>(this.baseUrl, category).pipe(
      map(res => res.data),
      catchError(this.handleError)
    );
  }

  updateCategory(id: string, category: Omit<ServiceCategory, 'id' | 'createdAt'>): Observable<ServiceCategory> {
    return this.http.put<ApiResponse<ServiceCategory>>(`${this.baseUrl}/${id}`, category).pipe(
      map(res => res.data),
      catchError(this.handleError)
    );
  }

  deleteCategory(id: string): Observable<void> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/${id}`).pipe(
      map(() => void 0),
      catchError(this.handleError)
    );
  }

  private handleError(error: any) {
    const msg = error.error?.message || error.message || 'Unknown error';
    console.error('API Error:', msg);
    return throwError(() => new Error(msg));
  }
}
