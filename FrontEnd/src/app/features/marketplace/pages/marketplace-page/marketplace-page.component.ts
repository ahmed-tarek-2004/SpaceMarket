import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FiltersComponent } from '../../components/filters/filters.component';
import { ServiceCardComponent } from '../../components/service-card/service-card.component';
import { DatasetCardComponent } from '../../components/dataset-card/dataset-card.component';
import { Observable } from 'rxjs';
import { MarketplaceFacadeService } from '../../services/marketplace-facade.service';
import { ApiServiceItem } from '../../interfaces/api-service-item';
import { ApiDatasetItem } from '../../interfaces/api-dataset-item';

@Component({
  selector: 'app-marketplace-page',
  standalone: true,
  imports: [CommonModule, FiltersComponent, ServiceCardComponent, DatasetCardComponent],
  templateUrl: './marketplace-page.component.html',
  styleUrls: ['./marketplace-page.component.scss'],
})
export class MarketplacePageComponent implements OnInit {
  facade = inject(MarketplaceFacadeService);

  activeTab: 'services' | 'datasets' = 'services';

  services$: Observable<ApiServiceItem[]>;
  datasets$: Observable<ApiDatasetItem[]>;

  loading$ = this.facade.loading$;
  error$ = this.facade.error$;

  constructor() {
    this.services$ = this.facade.services$;
    this.datasets$ = this.facade.datasets$;
  }

  ngOnInit(): void {
    this.facade.loadServices({ PageNumber: 1, PageSize: 12 });
    this.facade.loadDatasets({ PageNumber: 1, PageSize: 12 });
  }

  onFiltersChanged(query: any) {
    if (this.activeTab === 'services') {
      this.facade.loadServices({ ...query, PageNumber: 1 });
    } else {
      this.facade.loadDatasets({ ...query, PageNumber: 1 });
    }
  }

  switchTab(tab: 'services' | 'datasets') {
    this.activeTab = tab;

    if (tab === 'services') {
      this.facade.loadServices({ PageNumber: 1, PageSize: 12 });
    } else {
      this.facade.loadDatasets({ PageNumber: 1, PageSize: 12 });
    }
  }

  prev() {
    if (this.activeTab === 'services' && this.facade.pageNumber > 1) {
      this.facade.loadServices({ PageNumber: this.facade.pageNumber - 1 });
    } else if (this.activeTab === 'datasets' && this.facade.datasetApi.pageNumber > 1) {
      this.facade.loadDatasets({ PageNumber: this.facade.datasetApi.pageNumber - 1 });
    }
  }

  next() {
    if (this.activeTab === 'services' && this.facade.pageNumber < this.facade.totalPages) {
      this.facade.loadServices({ PageNumber: this.facade.pageNumber + 1 });
    } else if (
      this.activeTab === 'datasets' &&
      this.facade.datasetApi.pageNumber < this.facade.datasetApi.totalPages
    ) {
      this.facade.loadDatasets({ PageNumber: this.facade.datasetApi.pageNumber + 1 });
    }
  }

  get hasNextPage(): boolean {
    return this.activeTab === 'services'
      ? this.facade.pageNumber < this.facade.totalPages
      : this.facade.datasetApi.pageNumber < this.facade.datasetApi.totalPages;
  }

  get hasPrevPage(): boolean {
    return this.activeTab === 'services'
      ? this.facade.pageNumber > 1
      : this.facade.datasetApi.pageNumber > 1;
  }
}
