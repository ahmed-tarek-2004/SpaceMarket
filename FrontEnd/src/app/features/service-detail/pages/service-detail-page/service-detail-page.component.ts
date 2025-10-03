import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { ServiceDetailsResponse } from '../../interfaces/service.interface';
import { ServiceDetailService } from '../../services/service-detail.service';
import { ServiceHeaderComponent } from '../../components/service-header/service-header.component';
import { ServicePreviewComponent } from '../../components/service-preview/service-preview.component';
import { ServicePricingComponent } from '../../components/service-pricing/service-pricing.component';
import { CartFacadeService } from '../../../cart/services/cart-facade.service';

@Component({
  selector: 'app-service-detail',
  imports: [ServiceHeaderComponent, ServicePreviewComponent, ServicePricingComponent, CommonModule],
  templateUrl: './service-detail-page.component.html',
  styleUrls: ['./service-detail-page.component.scss'],
})
export class ServiceDetailPageComponent implements OnInit {
  service$!: Observable<ServiceDetailsResponse>;
  serviceId: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private serviceDetailService: ServiceDetailService,
    private cartFacade: CartFacadeService
  ) {}

  ngOnInit() {
    this.serviceId = this.route.snapshot.paramMap.get('id') || '';
    this.loadService();

    // Load cart on init
    this.cartFacade.loadCart();
  }

  loadService() {
    this.service$ = this.serviceDetailService.getService(this.serviceId);
  }

  goBack() {
    this.router.navigate(['/marketplace']);
  }
}
