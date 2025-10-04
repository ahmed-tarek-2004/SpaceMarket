import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { DebrisApiServiceService } from './debris-api-service.service';
import { SatelliteCard, SatelliteCardData } from '../interfaces/satellite-card';

@Injectable({
  providedIn: 'root',
})
export class AllSatellitesFacadeService {
  private debrisApiService = inject(DebrisApiServiceService);

  // Private subjects
  private _satellites$ = new BehaviorSubject<SatelliteCardData[]>([]);
  private _loading$ = new BehaviorSubject<boolean>(false);
  private _error$ = new BehaviorSubject<string | null>(null);

  // Public observables
  readonly satellites$ = this._satellites$.asObservable();
  readonly loading$ = this._loading$.asObservable();
  readonly error$ = this._error$.asObservable();

  // Pagination state
  pageNumber = 1;
  totalPages = 1;
  pageSize = 12;

  // Search state
  private _searchQuery$ = new BehaviorSubject<string>('');
  readonly searchQuery$ = this._searchQuery$.asObservable();

  loadSatellites(pageNumber: number = 1, searchQuery: string = '') {
    this._loading$.next(true);
    this._error$.next(null);

    console.log('Loading satellites - page:', pageNumber, 'search:', searchQuery);

    this.debrisApiService.getAllSatellites(pageNumber, this.pageSize, searchQuery).subscribe({
      next: (response) => {
        console.log('All satellites response:', response);
        this._loading$.next(false);

        if (response.succeeded && response.data) {
          // Use server-side pagination and search data directly
          this.pageNumber = response.data.pageNumber || pageNumber;
          this.totalPages = response.data.totalPages || 1;

          // Use satellites directly from server response
          const satellites = response.data.items || [];

          this._satellites$.next(satellites);
          console.log(
            'Satellites loaded:',
            satellites.length,
            'Total pages:',
            this.totalPages,
            'Current page:',
            this.pageNumber,
            'Page size:',
            this.pageSize,
            'Search query:',
            searchQuery
          );
        } else {
          this._error$.next(response.message || 'Failed to load satellites');
          this._satellites$.next([]);
        }
      },
      error: (error) => {
        this._loading$.next(false);
        this._error$.next('Failed to load satellites. Please try again.');
        this._satellites$.next([]);
        console.error('Error loading satellites:', error);
      },
    });
  }

  setSearchQuery(query: string) {
    this._searchQuery$.next(query);
    this.loadSatellites(1, query); // Reset to page 1 when searching
  }

  nextPage() {
    if (this.pageNumber < this.totalPages) {
      const currentSearchQuery = this._searchQuery$.value;
      this.loadSatellites(this.pageNumber + 1, currentSearchQuery);
    }
  }

  prevPage() {
    if (this.pageNumber > 1) {
      const currentSearchQuery = this._searchQuery$.value;
      this.loadSatellites(this.pageNumber - 1, currentSearchQuery);
    }
  }

  get hasNextPage(): boolean {
    return this.pageNumber < this.totalPages;
  }

  get hasPrevPage(): boolean {
    return this.pageNumber > 1;
  }

  refresh() {
    const currentSearchQuery = this._searchQuery$.value;
    this.loadSatellites(this.pageNumber, currentSearchQuery);
  }
}
