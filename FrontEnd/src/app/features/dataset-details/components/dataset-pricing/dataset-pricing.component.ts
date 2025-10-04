import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DatasetDetailsResponse } from '../../interfaces/dataset-details-response';
import { ROUTES } from '../../../../shared/config/constants';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-dataset-pricing',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dataset-pricing.component.html',
  styleUrls: ['./dataset-pricing.component.scss'],
})
export class DatasetPricingComponent {
  readonly ROUTES = ROUTES;

  @Input() dataset!: DatasetDetailsResponse;
  @Output() requestService = new EventEmitter<void>();
  @Output() contactProvider = new EventEmitter<void>();

  getProviderInitial(): string {
    return this.dataset.providerId?.charAt(0).toUpperCase() || 'P';
  }

  getStatusText(): string {
    return this.dataset.status || 'Active';
  }

  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';

    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  onRequestService() {
    this.requestService.emit();
  }

  onContactProvider() {
    this.contactProvider.emit();
  }
}
