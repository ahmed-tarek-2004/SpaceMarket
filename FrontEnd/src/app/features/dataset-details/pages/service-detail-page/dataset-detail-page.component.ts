import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CartFacadeService } from '../../../cart/services/cart-facade.service';
import { DatasetHeaderComponent } from '../../components/dataset-header/dataset-header.component';
import { DatasetPreviewComponent } from '../../components/dataset-preview/dataset-preview.component';
import { DatasetDetailsResponse } from '../../interfaces/dataset-details-response';
import { DatasetPricingComponent } from '../../components/dataset-pricing/dataset-pricing.component';
import { DatasetDetailService } from '../../services/dataset-detail.service';

@Component({
  selector: 'app-dataset-detail',
  standalone: true,
  imports: [DatasetHeaderComponent, DatasetPreviewComponent, DatasetPricingComponent, CommonModule],
  templateUrl: './dataset-detail-page.component.html',
  styleUrls: ['./dataset-detail-page.component.scss'],
})
export class DatasetDetailPageComponent implements OnInit {
  dataset$!: Observable<DatasetDetailsResponse>;
  datasetId: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private datasetDetailService: DatasetDetailService,
    private cartFacade: CartFacadeService
  ) {}

  ngOnInit() {
    this.datasetId = this.route.snapshot.paramMap.get('id') || '';
    this.loadDataset();
    this.cartFacade.loadCart();
  }

  loadDataset() {
    this.dataset$ = this.datasetDetailService.getDataset(this.datasetId);
  }

  onAddToCart(datasetId: string) {
    const request = {
      serviceId: undefined,
      dataSetId: datasetId,
    };

    this.cartFacade.addToCart(request);

    setTimeout(() => {
      this.router.navigate(['/cart']);
    }, 1500);
  }

  goBack() {
    this.router.navigate(['/marketplace']);
  }
}
