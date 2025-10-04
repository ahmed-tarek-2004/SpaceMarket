import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { DatasetResponse } from '../../interfaces/dataset/dataset-response';
import { UpdateRequest } from '../../interfaces/dataset/update-request'; // Import the UpdateRequest interface
import { ROUTES } from '../../../../../shared/config/constants';
import { ApiResponse } from '../../../../cart-items/interfaces/api-response';
import { ToastService } from '../../../../../shared/services/toast.service';
import { AppendToBodyDirective } from '../../../../../shared/directives/append-to-body.directive';
import { ProviderService } from '../../services/provider.service';

@Component({
  selector: 'app-provider-datasets',
  imports: [FormsModule, CommonModule, RouterModule, AppendToBodyDirective],
  templateUrl: './provider-datasets.component.html',
  styleUrls: ['./provider-datasets.component.scss'],
})
export class ProviderDatasetsComponent implements OnInit {
  readonly ROUTES = ROUTES;

  loading = false;
  datasets: (DatasetResponse & { showDetails?: boolean; isEditing?: boolean; editData?: any })[] =
    [];
  showCreateModal = false;
  dataFile: File | null = null;
  thumbnailFile: File | null = null;
  updateInProgress: { [key: string]: boolean } = {};
  deleteInProgress: { [key: string]: boolean } = {};

  // Delete confirmation modal properties
  showDeleteModal = false;
  datasetToDelete: DatasetResponse | null = null;

  constructor(
    private cdr: ChangeDetectorRef,
    private providerService: ProviderService,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    this.fetchDatasets();
  }

  fetchDatasets() {
    this.loading = true;
    this.providerService
      .getMyDatasets()
      .pipe(
        finalize(() => {
          this.loading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (response: ApiResponse<DatasetResponse[]>) => {
          if (response.succeeded) {
            this.datasets = response.data;
            this.initializeDatasetProperties();
          } else {
            console.error('Error fetching datasets:', response.message);
            this.datasets = [];
            this.toastService.error(response.message || 'Failed to load datasets');
          }
        },
        error: (error) => {
          console.error('Error fetching datasets:', error);
          this.datasets = [];
          this.toastService.error('An error occurred while loading datasets');
        },
      });
  }

  private initializeDatasetProperties() {
    this.datasets.forEach((dataset) => {
      dataset.showDetails = false;
      dataset.isEditing = false;
      dataset.editData = null;
    });
  }

  toggleDetails(dataset: DatasetResponse & { showDetails?: boolean }) {
    dataset.showDetails = !dataset.showDetails;
    this.cdr.detectChanges();
  }

  startEdit(dataset: DatasetResponse & { isEditing?: boolean; editData?: any }) {
    dataset.isEditing = true;
    dataset.editData = {
      title: dataset.title,
      description: dataset.description,
      price: dataset.price,
      categoryId: dataset.categoryId,
      apiEndpoint: dataset.apiEndpoint || '',
    };
    this.cdr.detectChanges();
  }

  cancelEdit(dataset: DatasetResponse & { isEditing?: boolean; editData?: any }) {
    dataset.isEditing = false;
    dataset.editData = null;
    this.dataFile = null;
    this.thumbnailFile = null;
    this.cdr.detectChanges();
  }

  async updateDataset(dataset: DatasetResponse & { isEditing?: boolean; editData?: any }) {
    if (!dataset.editData?.title?.trim()) {
      this.toastService.warning('Dataset title is required');
      return;
    }

    if (!dataset.editData?.description?.trim()) {
      this.toastService.warning('Dataset description is required');
      return;
    }

    if (dataset.editData?.price < 0) {
      this.toastService.warning('Price cannot be negative');
      return;
    }

    if (dataset.editData?.apiEndpoint && !this.isValidUrl(dataset.editData.apiEndpoint)) {
      this.toastService.warning('Please enter a valid API endpoint URL');
      return;
    }

    this.updateInProgress[dataset.id] = true;

    try {
      // Prepare the update request according to the UpdateRequest interface
      const updateRequest: UpdateRequest = {
        id: dataset.id,
        title: dataset.editData.title,
        description: dataset.editData.description,
        price: dataset.editData.price,
        categoryId: dataset.editData.categoryId,
        apiEndpoint: dataset.editData.apiEndpoint || undefined,
        file: await this.getFileAsBase64(this.dataFile, dataset.fileUrl), // Required field
        thumbnail: await this.getFileAsBase64(this.thumbnailFile, dataset.thumbnailUrl), // Optional field
      };

      // Remove undefined fields to avoid sending them
      Object.keys(updateRequest).forEach((key) => {
        if (updateRequest[key as keyof UpdateRequest] === undefined) {
          delete updateRequest[key as keyof UpdateRequest];
        }
      });

      this.providerService.updateDataset(updateRequest).subscribe({
        next: (response: ApiResponse<DatasetResponse>) => {
          this.updateInProgress[dataset.id] = false;

          if (response.succeeded) {
            // Update local dataset with response data
            const updatedDataset = response.data;
            Object.assign(dataset, updatedDataset);

            dataset.isEditing = false;
            dataset.editData = null;
            this.dataFile = null;
            this.thumbnailFile = null;

            this.toastService.success(`"${dataset.title}" updated successfully`);
            console.log(`Dataset ${dataset.id} updated successfully`);
          } else {
            const errorMessage = response.message || 'Failed to update dataset';
            console.error('Error updating dataset:', errorMessage);
            this.toastService.error(errorMessage);
          }
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.updateInProgress[dataset.id] = false;
          console.error('Error updating dataset:', error);
          this.toastService.error('An error occurred while updating the dataset');
          this.cdr.detectChanges();
        },
      });
    } catch (error) {
      this.updateInProgress[dataset.id] = false;
      console.error('Error preparing update request:', error);
      this.toastService.error('An error occurred while preparing the update');
      this.cdr.detectChanges();
    }
  }

  private async getFileAsBase64(file: File | null, existingUrl: string): Promise<string> {
    if (file) {
      // Convert new file to base64
      return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
          const base64 = reader.result as string;
          // Remove the data URL prefix if present
          const base64Data = base64.split(',')[1] || base64;
          resolve(base64Data);
        };
        reader.onerror = reject;
        reader.readAsDataURL(file);
      });
    } else if (existingUrl && this.hasValidFile(existingUrl)) {
      // If no new file but existing file, we need to handle this case
      // You might want to fetch the existing file and convert to base64
      // or modify the backend to handle updates without re-uploading files
      // For now, we'll return the existing URL or empty string
      // You may need to adjust this based on your backend requirements
      return existingUrl;
    }

    // Return empty string if no file is provided and no existing file
    return '';
  }

  private isValidUrl(string: string): boolean {
    try {
      new URL(string);
      return true;
    } catch (_) {
      return false;
    }
  }

  // Open delete confirmation modal
  openDeleteModal(dataset: DatasetResponse) {
    this.datasetToDelete = dataset;
    this.showDeleteModal = true;
    this.cdr.detectChanges();
  }

  // Close delete confirmation modal
  closeDeleteModal() {
    this.showDeleteModal = false;
    this.datasetToDelete = null;
    this.cdr.detectChanges();
  }

  // Confirm and execute deletion
  confirmDelete() {
    if (!this.datasetToDelete) return;

    const dataset = this.datasetToDelete;
    this.deleteInProgress[dataset.id] = true;

    this.providerService.deleteDataset(dataset.id).subscribe({
      next: (response: ApiResponse<boolean>) => {
        this.deleteInProgress[dataset.id] = false;

        if (response.succeeded) {
          // Remove dataset from local list
          this.datasets = this.datasets.filter((d) => d.id !== dataset.id);
          this.toastService.success(`"${dataset.title}" deleted successfully`);
          console.log(`Dataset ${dataset.id} deleted successfully`);
        } else {
          const errorMessage = response.message || 'Failed to delete dataset';
          console.error('Error deleting dataset:', errorMessage);
          this.toastService.error(errorMessage);
        }
        this.closeDeleteModal();
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.deleteInProgress[dataset.id] = false;
        console.error('Error deleting dataset:', error);
        this.toastService.error('An error occurred while deleting the dataset');
        this.closeDeleteModal();
        this.cdr.detectChanges();
      },
    });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      // Validate file type and size
      const validFileTypes = ['image/jpeg', 'image/png', 'image/webp'];
      const maxSize = 5 * 1024 * 1024;

      const fileExtension = '.' + file.name.split('.').pop().toLowerCase();
      if (!validFileTypes.includes(fileExtension)) {
        this.toastService.error('Please select a valid dataset file (JPEG, PNG, or WebP)');
        event.target.value = '';
        return;
      }

      if (file.size > maxSize) {
        this.toastService.error('File size must be less than 5MB');
        event.target.value = '';
        return;
      }

      this.dataFile = file;
      this.toastService.success('Dataset file selected successfully');
    }
  }

  onThumbnailSelected(event: any) {
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

      this.thumbnailFile = file;
      this.toastService.success('Thumbnail image selected successfully');
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

  hasValidFile(fileUrl: string): boolean {
    return !!fileUrl && fileUrl.trim() !== '' && fileUrl !== 'null';
  }

  hasValidThumbnail(thumbnailUrl: string): boolean {
    return !!thumbnailUrl && thumbnailUrl.trim() !== '' && thumbnailUrl !== 'null';
  }

  getFileName(fileUrl: string): string {
    if (!this.hasValidFile(fileUrl)) return '';
    return fileUrl.split('/').pop() || 'dataset file';
  }

  isUpdating(datasetId: string): boolean {
    return this.updateInProgress[datasetId] || false;
  }

  isDeleting(datasetId: string): boolean {
    return this.deleteInProgress[datasetId] || false;
  }
}
