import { Component, OnInit, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize, take } from 'rxjs/operators';

import { ServiceListService } from '../../services/service-list.service';
import { ServiceListFilterRequest } from '../../interfaces/service/service-list-filter-request';
import { UpdateServiceStatusRequest } from '../../interfaces/service/update-service-status-request';
import { ServiceResponse } from '../../interfaces/service/service-response';

@Component({
  selector: 'app-service-list-admin',
  imports: [FormsModule, CommonModule],
  templateUrl: './service-list-admin.component.html',
  styleUrls: ['./service-list-admin.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServiceListAdminComponent implements OnInit {
  loading = false;
  services: ServiceResponse[] = [];

  filters: ServiceListFilterRequest = {
    categoryId: '',
    providerId: '',
    status: '',
  };

  constructor(private serviceListService: ServiceListService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.fetchServices();
  }

  fetchServices() {
    this.loading = true;
    this.serviceListService
      .getServices(this.filters)
      .pipe(
        take(1),
        finalize(() => {
          this.loading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (response) => {
          if (response.succeeded) {
            this.services = response.data;
            this.initializeServiceProperties();
            this.cdr.detectChanges();
          } else {
            console.error('Error fetching services:', response.message);
            this.services = [];
          }
        },
        error: (error) => {
          console.error('Error fetching services:', error);
          this.services = [];
        },
      });
  }

  private initializeServiceProperties() {
    this.services.forEach((service) => {
      service.newStatus = '';
      service.reason = '';
      service.showReasonInput = false;
      service.showDetails = false;
    });
  }

  toggleDetails(service: ServiceResponse) {
    service.showDetails = !service.showDetails;
    this.cdr.detectChanges();
  }

  onStatusChange(service: ServiceResponse) {
    if (service.newStatus && service.newStatus !== service.status) {
      service.showReasonInput = true;
      service.reason = '';
    } else {
      service.showReasonInput = false;
    }
    this.cdr.detectChanges();
  }

  updateServiceStatus(service: ServiceResponse) {
    if (!service.reason?.trim()) return;

    const updateData: UpdateServiceStatusRequest = {
      serviceId: service.id,
      status: service.newStatus!,
      reason: service.reason,
    };

    this.serviceListService.updateServiceStatus(updateData).subscribe({
      next: (response) => {
        if (response.succeeded) {
          service.status = service.newStatus!;
          service.newStatus = '';
          service.reason = '';
          service.showReasonInput = false;
          console.log(`Service ${service.id} status updated to ${service.status}`);
          this.cdr.markForCheck();
        } else {
          console.error('Error updating service status:', response.message);
        }
      },
      error: (error) => {
        console.error('Error updating service status:', error);
      },
    });
  }

  cancelStatusUpdate(service: ServiceResponse) {
    service.newStatus = '';
    service.reason = '';
    service.showReasonInput = false;
    this.cdr.detectChanges();
  }

  getStatusClasses(status: string): string {
    switch (status) {
      case 'Active':
        return 'bg-green-500/20 text-green-400';
      case 'PendingApproval':
        return 'bg-yellow-500/20 text-yellow-400';
      case 'Suspended':
        return 'bg-red-500/20 text-red-400';
      default:
        return 'bg-gray-500/20 text-gray-400';
    }
  }

  // Helper method to check if image URL is valid
  hasValidImage(imagesUrl: string): boolean {
    return !!imagesUrl && imagesUrl.trim() !== '' && imagesUrl !== 'null';
  }
}
