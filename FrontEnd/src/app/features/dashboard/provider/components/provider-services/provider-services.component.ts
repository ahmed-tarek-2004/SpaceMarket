import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { ServiceResponse } from '../../interfaces/service/service-response';
import { ROUTES } from '../../../../../shared/config/constants';
import { ProviderService } from '../../services/provider.service';
import { ApiResponse } from '../../../../cart-items/interfaces/api-response';
import { ToastService } from '../../../../../shared/services/toast.service';
import { AppendToBodyDirective } from '../../../../../shared/directives/append-to-body.directive';

@Component({
  selector: 'app-provider-services',
  imports: [FormsModule, CommonModule, RouterModule, AppendToBodyDirective],
  templateUrl: './provider-services.component.html',
  styleUrls: ['./provider-services.component.scss'],
})
export class ProviderServicesComponent implements OnInit {
  readonly ROUTES = ROUTES;

  loading = false;
  services: ServiceResponse[] = [];
  showCreateModal = false;
  imageFile: File | null = null;
  updateInProgress: { [key: string]: boolean } = {};
  deleteInProgress: { [key: string]: boolean } = {};

  // Delete confirmation modal properties
  showDeleteModal = false;
  serviceToDelete: ServiceResponse | null = null;

  constructor(
    private cdr: ChangeDetectorRef,
    private providerService: ProviderService,
    private toastService: ToastService // Inject ToastService
  ) {}

  ngOnInit() {
    this.fetchServices();
  }

  fetchServices() {
    this.loading = true;
    this.providerService
      .getMyServices()
      .pipe(
        finalize(() => {
          this.loading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (response: ApiResponse<ServiceResponse[]>) => {
          if (response.succeeded) {
            this.services = response.data;
            this.initializeServiceProperties();
          } else {
            console.error('Error fetching services:', response.message);
            this.services = [];
            this.toastService.error(response.message || 'Failed to load services');
          }
        },
        error: (error) => {
          console.error('Error fetching services:', error);
          this.services = [];
          this.toastService.error('An error occurred while loading services');
        },
      });
  }

  private initializeServiceProperties() {
    this.services.forEach((service) => {
      service.showDetails = false;
      service.isEditing = false;
      service.editData = null;
    });
  }

  toggleDetails(service: ServiceResponse) {
    service.showDetails = !service.showDetails;
    this.cdr.detectChanges();
  }

  startEdit(service: ServiceResponse) {
    service.isEditing = true;
    service.editData = {
      title: service.title,
      description: service.description,
      price: service.price,
    };
    this.cdr.detectChanges();
  }

  cancelEdit(service: ServiceResponse) {
    service.isEditing = false;
    service.editData = null;
    this.cdr.detectChanges();
  }

  updateService(service: ServiceResponse) {
    if (!service.editData?.title?.trim()) {
      this.toastService.warning('Service title is required');
      return;
    }

    this.updateInProgress[service.id] = true;

    // Create FormData for the update request
    const formData = new FormData();

    // Append all required fields
    formData.append('id', service.id);
    formData.append('title', service.editData.title);
    formData.append('description', service.editData.description || '');
    formData.append('price', service.editData.price.toString());

    // Append image if selected
    if (this.imageFile) {
      formData.append('image', this.imageFile);
    }

    this.providerService.updateService(formData).subscribe({
      next: (response: ApiResponse<ServiceResponse>) => {
        this.updateInProgress[service.id] = false;

        if (response.succeeded) {
          // Update local service with response data
          const updatedService = response.data;
          Object.assign(service, updatedService);

          service.isEditing = false;
          service.editData = null;
          this.imageFile = null;

          this.toastService.success(`"${service.title}" updated successfully`);
          console.log(`Service ${service.id} updated successfully`);
        } else {
          const errorMessage = response.message || 'Failed to update service';
          console.error('Error updating service:', errorMessage);
          this.toastService.error(errorMessage);
        }
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.updateInProgress[service.id] = false;
        console.error('Error updating service:', error);
        this.toastService.error('An error occurred while updating the service');
        this.cdr.detectChanges();
      },
    });
  }

  // Open delete confirmation modal
  openDeleteModal(service: ServiceResponse) {
    this.serviceToDelete = service;
    this.showDeleteModal = true;
    this.cdr.detectChanges();
  }

  // Close delete confirmation modal
  closeDeleteModal() {
    this.showDeleteModal = false;
    this.serviceToDelete = null;
    this.cdr.detectChanges();
  }

  // Confirm and execute deletion
  confirmDelete() {
    if (!this.serviceToDelete) return;

    const service = this.serviceToDelete;
    this.deleteInProgress[service.id] = true;

    this.providerService.deleteService(service.id).subscribe({
      next: (response: ApiResponse<boolean>) => {
        this.deleteInProgress[service.id] = false;

        if (response.succeeded) {
          // Remove service from local list
          this.services = this.services.filter((s) => s.id !== service.id);
          this.toastService.success(`"${service.title}" deleted successfully`);
          console.log(`Service ${service.id} deleted successfully`);
        } else {
          const errorMessage = response.message || 'Failed to delete service';
          console.error('Error deleting service:', errorMessage);
          this.toastService.error(errorMessage);
        }
        this.closeDeleteModal();
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.deleteInProgress[service.id] = false;
        console.error('Error deleting service:', error);
        this.toastService.error('An error occurred while deleting the service');
        this.closeDeleteModal();
        this.cdr.detectChanges();
      },
    });
  }

  onImageSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      // Validate file type and size
      const validImageTypes = ['image/jpeg', 'image/png', 'image/webp'];
      const maxSize = 5 * 1024 * 1024; // 5MB

      if (!validImageTypes.includes(file.type)) {
        this.toastService.error('Please select a valid image file (JPEG, PNG, or WebP)');
        event.target.value = '';
        return;
      }

      if (file.size > maxSize) {
        this.toastService.error('Image size must be less than 5MB');
        event.target.value = '';
        return;
      }

      this.imageFile = file;
      this.toastService.success('Image selected successfully');
    }
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

  hasValidImage(imagesUrl: string): boolean {
    return !!imagesUrl && imagesUrl.trim() !== '' && imagesUrl !== 'null';
  }

  isUpdating(serviceId: string): boolean {
    return this.updateInProgress[serviceId] || false;
  }

  isDeleting(serviceId: string): boolean {
    return this.deleteInProgress[serviceId] || false;
  }
}
