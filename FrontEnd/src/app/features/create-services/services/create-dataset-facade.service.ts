import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap, catchError, throwError } from 'rxjs';
import { CreateDatasetRequest } from '../interfaces/create-dataset-request';
import { CreateDatasetApiService } from './create-dataset-api.service';

@Injectable({
  providedIn: 'root',
})
export class CreateDatasetFacadeService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  private errorSubject = new BehaviorSubject<string | null>(null);

  public loading$ = this.loadingSubject.asObservable();
  public error$ = this.errorSubject.asObservable();

  constructor(private createDatasetApi: CreateDatasetApiService) {}

  createDataset(
    createDatasetRequest: CreateDatasetRequest,
    file: File,
    thumbnail?: File
  ): Observable<any> {
    this.loadingSubject.next(true);
    this.errorSubject.next(null);

    return this.createDatasetApi.createDataset(createDatasetRequest, file, thumbnail).pipe(
      tap((response) => {
        this.loadingSubject.next(false);
        console.log('Dataset created successfully:', response);
      }),
      catchError((error) => {
        this.loadingSubject.next(false);
        const errorMessage = this.getErrorMessage(error);
        this.errorSubject.next(errorMessage);
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  private getErrorMessage(error: any): string {
    if (error.error?.message) {
      return error.error.message;
    }
    if (error.status === 401) {
      return 'Unauthorized. Please check your authentication.';
    }
    if (error.status === 400) {
      return 'Invalid request. Please check your input.';
    }
    if (error.status === 413) {
      return 'File too large. Please check file size limits.';
    }
    if (error.status === 404) {
      return 'Service not found. Please check the category.';
    }
    return 'An unexpected error occurred. Please try again.';
  }

  clearError(): void {
    this.errorSubject.next(null);
  }
}
