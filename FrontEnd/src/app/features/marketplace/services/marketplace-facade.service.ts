import { Injectable } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { MarketplaceApiService } from './marketplace-api.service';
import { ApiServiceItem } from '../interfaces/api-service-item';
import { ServiceQuery } from '../interfaces/service-query';
import { ApiDatasetItem } from '../interfaces/api-dataset-item';

@Injectable({
  providedIn: 'root',
})
export class MarketplaceFacadeService {
  private _services$ = new BehaviorSubject<ApiServiceItem[]>([]);
  readonly services$ = this._services$.asObservable();

  private _loading$ = new BehaviorSubject<boolean>(false);
  readonly loading$ = this._loading$.asObservable();

  private _error$ = new BehaviorSubject<string | null>(null);
  readonly error$ = this._error$.asObservable();

  private currentSubscription?: Subscription;

  pageNumber = 1;
  totalPages = 1;

  constructor(private api: MarketplaceApiService) {}

  loadServices(query?: ServiceQuery) {
    this.currentSubscription?.unsubscribe();
    this._loading$.next(true);
    this._error$.next(null);

    const payload: ServiceQuery = {
      PageNumber: query?.PageNumber ?? this.pageNumber,
      PageSize: query?.PageSize,
      CategoryId: query?.CategoryId,
      MinPrice: query?.MinPrice,
      MaxPrice: query?.MaxPrice,
      Location: query?.Location,
    };

    this.currentSubscription = this.api.getAvailableServices(payload).subscribe({
      next: (paged) => {
        this._services$.next(paged.items || []);
        this.pageNumber = paged.pageNumber ?? 1;
        this.totalPages = paged.totalPages ?? 1;
        this._loading$.next(false);
      },
      error: (err) => {
        console.error('MarketplaceFacadeService.loadServices error', err);
        this._error$.next('Failed to load services');
        this._services$.next([]);
        this._loading$.next(false);
      },
    });
  }

  refresh() {
    this.loadServices({ PageNumber: this.pageNumber });
  }

  // =================

  private _datasets$ = new BehaviorSubject<ApiDatasetItem[]>([]);
  readonly datasets$ = this._datasets$.asObservable();

  datasetApi = { pageNumber: 1, totalPages: 1 };

  loadDatasets(query?: ServiceQuery) {
    this.currentSubscription?.unsubscribe();
    this._loading$.next(true);
    this._error$.next(null);

    const payload: ServiceQuery = {
      PageNumber: query?.PageNumber ?? this.datasetApi.pageNumber,
      PageSize: query?.PageSize,
      CategoryId: query?.CategoryId,
      MinPrice: query?.MinPrice,
      MaxPrice: query?.MaxPrice,
      Location: query?.Location,
    };

    this.currentSubscription = this.api.getAvailableDatasets(payload).subscribe({
      next: (paged) => {
        this._datasets$.next(paged.items || []);
        this.datasetApi.pageNumber = paged.pageNumber ?? 1;
        this.datasetApi.totalPages = paged.totalPages ?? 1;
        this._loading$.next(false);
      },
      error: (err) => {
        console.error('MarketplaceFacadeService.loadDatasets error', err);
        this._error$.next('Failed to load datasets');
        this._datasets$.next([]);
        this._loading$.next(false);
      },
    });
  }
}
