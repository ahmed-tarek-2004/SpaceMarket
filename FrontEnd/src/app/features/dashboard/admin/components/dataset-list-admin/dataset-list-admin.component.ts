import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize, take } from 'rxjs/operators';
import { DatasetListService } from '../../services/dataset-list.service';
import { DatasetListFilterRequest } from '../../interfaces/dataset/dataset-list-filter-request';
import { DatasetResponse } from '../../interfaces/dataset/dataset-response';
import { UpdateDatasetStatusRequest } from '../../interfaces/dataset/update-dataset-status-request';

@Component({
  selector: 'app-dataset-list-admin',
  imports: [FormsModule, CommonModule],
  templateUrl: './dataset-list-admin.component.html',
  styleUrls: ['./dataset-list-admin.component.scss'],
})
export class DatasetListAdminComponent implements OnInit {
  loading = false;
  datasets: DatasetResponse[] = [];

  filters: DatasetListFilterRequest = {
    categoryId: '',
    providerId: '',
    status: '',
    minPrice: undefined,
    maxPrice: undefined,
    pageNumber: 1,
    pageSize: 50
  };

  constructor(private datasetListService: DatasetListService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.fetchDatasets();
  }

  fetchDatasets() {
    this.loading = true;
    this.datasetListService
      .getDatasets(this.filters)
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
            this.datasets = response.data;
            this.initializeDatasetProperties();
            this.cdr.detectChanges();
          } else {
            console.error('Error fetching datasets:', response.message);
            this.datasets = [];
          }
        },
        error: (error) => {
          console.error('Error fetching datasets:', error);
          this.datasets = [];
        },
      });
  }

  private initializeDatasetProperties() {
    this.datasets.forEach((dataset) => {
      dataset.newStatus = '';
      dataset.reason = '';
      dataset.showReasonInput = false;
      dataset.showDetails = false;
    });
  }

  toggleDetails(dataset: DatasetResponse) {
    dataset.showDetails = !dataset.showDetails;
    this.cdr.detectChanges();
  }

  onStatusChange(dataset: DatasetResponse) {
    if (dataset.newStatus && dataset.newStatus !== dataset.status) {
      dataset.showReasonInput = true;
      dataset.reason = '';
    } else {
      dataset.showReasonInput = false;
    }
    this.cdr.detectChanges();
  }

  updateDatasetStatus(dataset: DatasetResponse) {
    if (!dataset.reason?.trim()) return;

    const updateData: UpdateDatasetStatusRequest = {
      datasetId: dataset.id,
      status: dataset.newStatus!,
      reason: dataset.reason,
    };

    this.datasetListService.updateDatasetStatus(updateData).subscribe({
      next: (response) => {
        if (response.succeeded) {
          dataset.status = dataset.newStatus!;
          dataset.newStatus = '';
          dataset.reason = '';
          dataset.showReasonInput = false;
          console.log(`Dataset ${dataset.id} status updated to ${dataset.status}`);
          this.cdr.detectChanges();
        } else {
          console.error('Error updating dataset status:', response.message);
        }
      },
      error: (error) => {
        console.error('Error updating dataset status:', error);
      },
    });
  }

  cancelStatusUpdate(dataset: DatasetResponse) {
    dataset.newStatus = '';
    dataset.reason = '';
    dataset.showReasonInput = false;
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

  // Helper method to check if thumbnail URL is valid
  hasValidThumbnail(thumbnailUrl: string): boolean {
    return !!thumbnailUrl && thumbnailUrl.trim() !== '' && thumbnailUrl !== 'null';
  }

  // Helper to get provider display name
  getProviderDisplay(dataset: DatasetResponse): string {
    if (dataset.providerName && dataset.providerEmail) {
      return `${dataset.providerName}`;
    }
    return dataset.providerName || dataset.providerId || 'Unknown Provider';
  }
}