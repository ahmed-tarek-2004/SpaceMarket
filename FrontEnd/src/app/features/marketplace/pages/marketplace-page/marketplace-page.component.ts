import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FiltersComponent } from '../../components/filters/filters.component';
import { ServiceCardComponent } from '../../components/service-card/service-card.component';
import { Observable } from 'rxjs';
import { MarketplaceFacadeService } from '../../services/marketplace-facade.service';
import { ApiServiceItem } from '../../interfaces/api-service-item';

@Component({
  selector: 'app-marketplace-page',
  standalone: true,
  imports: [CommonModule, FiltersComponent, ServiceCardComponent],
  templateUrl: './marketplace-page.component.html',
  styleUrls: ['./marketplace-page.component.scss'],
})
export class MarketplacePageComponent implements OnInit {
  facade = inject(MarketplaceFacadeService);

  services$: Observable<ApiServiceItem[]>;
  loading$ = this.facade.loading$;
  error$ = this.facade.error$;

  constructor() {
    this.services$ = this.facade.services$;
  }

  ngOnInit(): void {
    this.facade.loadServices({ PageNumber: 1, PageSize: 12 });
  }

  onFiltersChanged(query: any) {
    this.facade.loadServices({ ...query, PageNumber: 1 });
  }

  prev() {
    if (this.facade.pageNumber > 1) {
      this.facade.loadServices({ PageNumber: this.facade.pageNumber - 1 });
    }
  }

  next() {
    if (this.facade.pageNumber < this.facade.totalPages) {
      this.facade.loadServices({ PageNumber: this.facade.pageNumber + 1 });
    }
  }

  get hasNextPage(): boolean {
    return this.facade.pageNumber < this.facade.totalPages;
  }

  get hasPrevPage(): boolean {
    return this.facade.pageNumber > 1;
  }
}
