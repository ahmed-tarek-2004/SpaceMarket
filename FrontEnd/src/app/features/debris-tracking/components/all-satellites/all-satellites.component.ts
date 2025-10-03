import { Component, Input, Output, EventEmitter, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Satellite } from '../../interfaces/satellite';
import { DebrisApiServiceService } from '../../services/debris-api-service.service';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-all-satellites',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './all-satellites.component.html',
  styleUrls: ['./all-satellites.component.scss'],
})
export class AllSatellitesComponent implements OnInit {
  @Output() satelliteSelected = new EventEmitter<{
    id: string;
    name: string;
    proximityThresholdKm: number;
  }>();

  private debrisApiService = inject(DebrisApiServiceService);
  private toastService = inject(ToastService);

  // Data properties
  allSatellites: Satellite[] = [];
  filteredSatellites: Satellite[] = [];

  // State signals
  isLoading = signal(false);
  hasError = signal(false);
  errorMessage = signal('');

  // Pagination properties
  currentPage = signal(1);
  itemsPerPage = signal(12);
  totalPages = signal(0);

  // Filter properties
  searchQuery = signal('');

  // Computed signals
  paginatedSatellites = signal<Satellite[]>([]);

  ngOnInit(): void {
    this.loadAllSatellites();
  }

  loadAllSatellites(): void {
    this.isLoading.set(true);
    this.hasError.set(false);
    this.errorMessage.set('');

    this.debrisApiService.getAllSatellites().subscribe({
      next: (response) => {
        this.isLoading.set(false);
        if (response.succeeded) {
          this.allSatellites = response.data || [];
          this.applyFilters();
        } else {
          this.hasError.set(true);
          this.errorMessage.set(response.message || 'Failed to load satellites');
        }
      },
      error: (error) => {
        this.isLoading.set(false);
        this.hasError.set(true);
        console.error('Error loading satellites:', error);

        if (error.error?.message) {
          this.errorMessage.set(error.error.message);
        } else if (error.message) {
          this.errorMessage.set(error.message);
        } else {
          this.errorMessage.set('Failed to load satellites. Please try again later.');
        }
      },
    });
  }

  applyFilters(): void {
    const query = this.searchQuery().toLowerCase().trim();

    if (query === '') {
      this.filteredSatellites = [...this.allSatellites];
    } else {
      this.filteredSatellites = this.allSatellites.filter(
        (satellite) =>
          satellite.name.toLowerCase().includes(query) ||
          satellite.id.toLowerCase().includes(query) ||
          satellite.noradId.toLowerCase().includes(query)
      );
    }

    this.updatePagination();
  }

  updatePagination(): void {
    const totalItems = this.filteredSatellites.length;
    const totalPages = Math.ceil(totalItems / this.itemsPerPage());

    this.totalPages.set(totalPages);

    // Reset to page 1 if current page exceeds total pages
    if (this.currentPage() > totalPages && totalPages > 0) {
      this.currentPage.set(1);
    }

    this.updatePaginatedData();
  }

  updatePaginatedData(): void {
    const startIndex = (this.currentPage() - 1) * this.itemsPerPage();
    const endIndex = startIndex + this.itemsPerPage();

    this.paginatedSatellites.set(this.filteredSatellites.slice(startIndex, endIndex));
  }

  onSearchChange(query: string): void {
    this.searchQuery.set(query);
    this.currentPage.set(1); // Reset to first page when searching
    this.applyFilters();
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.updatePaginatedData();
    }
  }

  onSatelliteClick(satellite: Satellite): void {
    this.satelliteSelected.emit({
      id: satellite.id,
      name: satellite.name,
      proximityThresholdKm: satellite.proximityThresholdKm,
    });
  }

  getStatusColor(threshold: number): string {
    if (threshold <= 5) return 'text-red-400';
    if (threshold <= 10) return 'text-yellow-400';
    return 'text-green-400';
  }

  getStatusText(threshold: number): string {
    if (threshold <= 5) return 'High Risk';
    if (threshold <= 10) return 'Medium Risk';
    return 'Low Risk';
  }

  getRelativeTime(date: Date): string {
    const now = new Date();
    const diffInHours = Math.floor((now.getTime() - new Date(date).getTime()) / (1000 * 60 * 60));

    if (diffInHours < 24) {
      return `${diffInHours}h ago`;
    }

    const diffInDays = Math.floor(diffInHours / 24);
    if (diffInDays < 7) {
      return `${diffInDays}d ago`;
    }

    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  trackBySatelliteId(index: number, satellite: Satellite): string {
    return satellite.id;
  }

  trackByPageNumber(index: number, page: number): number {
    return page;
  }

  // Expose Math to template
  Math = Math;

  // Pagination helper methods
  getPageNumbers(): number[] {
    const total = this.totalPages();
    const current = this.currentPage();
    const pages: number[] = [];

    // Always show first page
    if (total > 0) pages.push(1);

    // Show pages around current page
    const start = Math.max(2, current - 1);
    const end = Math.min(total - 1, current + 1);

    for (let i = start; i <= end; i++) {
      if (!pages.includes(i)) {
        pages.push(i);
      }
    }

    // Always show last page
    if (total > 1 && !pages.includes(total)) {
      pages.push(total);
    }

    return pages;
  }

  refreshSatellites(): void {
    this.loadAllSatellites();
  }
}
