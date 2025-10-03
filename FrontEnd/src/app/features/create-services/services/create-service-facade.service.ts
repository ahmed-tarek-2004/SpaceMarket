// create-service-facade.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap, catchError } from 'rxjs';
import { CreateServiceApiService } from './create-service-api.service';
import { CreateServiceRequest } from '../interfaces/create-service-request';

@Injectable({
  providedIn: 'root',
})
export class CreateServiceFacadeService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  private errorSubject = new BehaviorSubject<string | null>(null);

  public loading$ = this.loadingSubject.asObservable();
  public error$ = this.errorSubject.asObservable();

  constructor(private createServiceApi: CreateServiceApiService) {}

  createService(createServiceRequest: CreateServiceRequest): Observable<any> {
    this.loadingSubject.next(true);
    this.errorSubject.next(null);

    return this.createServiceApi.createService(createServiceRequest).pipe(
      tap((response) => {
        this.loadingSubject.next(false);
        console.log('Service created successfully:', response);
      }),
      catchError((error) => {
        this.loadingSubject.next(false);
        this.errorSubject.next(this.getErrorMessage(error));
        throw error;
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
    return 'An unexpected error occurred. Please try again.';
  }

  clearError(): void {
    this.errorSubject.next(null);
  }
}
