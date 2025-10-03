import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ServiceDetailsResponse } from '../../interfaces/service.interface';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ROUTES } from '../../../../shared/config/constants';

@Component({
  selector: 'app-service-pricing',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './service-pricing.component.html',
  styleUrls: ['./service-pricing.component.scss'],
})
export class ServicePricingComponent {
  readonly ROUTES = ROUTES;

  @Input() service!: ServiceDetailsResponse;
  @Output() requestService = new EventEmitter<void>();
  @Output() contactProvider = new EventEmitter<void>();

  getProviderInitial(): string {
    return this.service.providerId?.charAt(0).toUpperCase() || 'P';
  }

  getStatusText(): string {
    return this.service.status || 'Active';
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
