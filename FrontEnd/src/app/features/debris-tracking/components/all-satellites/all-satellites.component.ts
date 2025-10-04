import { Component, Input, Output, EventEmitter, OnInit, inject } from '@angular/core';
import { AsyncPipe, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SatelliteCardData } from '../../interfaces/satellite-card';
import { AllSatellitesFacadeService } from '../../services/all-satellites-facade.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-all-satellites',
  standalone: true,
  imports: [CommonModule, FormsModule, AsyncPipe],
  templateUrl: './all-satellites.component.html',
  styleUrls: ['./all-satellites.component.scss'],
})
export class AllSatellitesComponent implements OnInit {
  @Output() satelliteSelected = new EventEmitter<{
    id: string;
    name: string;
    noradId: string;
  }>();

  private facade = inject(AllSatellitesFacadeService);

  // Observables from facade
  satellites$: Observable<SatelliteCardData[]>;
  loading$: Observable<boolean>;
  error$: Observable<string | null>;
  searchQuery$: Observable<string>;

  // Pagination properties
  get pageNumber(): number {
    return this.facade.pageNumber;
  }

  get totalPages(): number {
    return this.facade.totalPages;
  }

  get hasNextPage(): boolean {
    return this.facade.hasNextPage;
  }

  get hasPrevPage(): boolean {
    return this.facade.hasPrevPage;
  }

  constructor() {
    this.satellites$ = this.facade.satellites$;
    this.loading$ = this.facade.loading$;
    this.error$ = this.facade.error$;
    this.searchQuery$ = this.facade.searchQuery$;
  }

  ngOnInit(): void {
    this.facade.loadSatellites();
  }

  onSearchChange(query: string): void {
    this.facade.setSearchQuery(query);
  }

  nextPage(): void {
    this.facade.nextPage();
  }

  prevPage(): void {
    this.facade.prevPage();
  }

  onSatelliteClick(satellite: SatelliteCardData): void {
    this.satelliteSelected.emit({
      id: satellite.id,
      name: satellite.name,
      noradId: satellite.noradId,
    });
  }

  getStatusColor(): string {
    return 'text-blue-400';
  }

  getStatusText(): string {
    return 'Available';
  }

  trackBySatelliteId(index: number, satellite: SatelliteCardData): string {
    return satellite.id;
  }

  refreshSatellites(): void {
    this.facade.refresh();
  }
}
