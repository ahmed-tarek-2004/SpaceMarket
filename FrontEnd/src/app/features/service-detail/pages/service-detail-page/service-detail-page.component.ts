// src/app/features/service-detail/pages/service-detail-page/service-detail-page.component.ts
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Service } from '../../interfaces/service.interface';
import { ServiceDetailService } from '../../services/service-detail.service';
import { ServiceHeaderComponent } from '../../components/service-header/service-header.component';
import { ServicePreviewComponent } from '../../components/service-preview/service-preview.component';
import { ServicePricingComponent } from '../../components/service-pricing/service-pricing.component';
import { ServiceDescriptionTabsComponent } from '../../components/service-description-tabs/service-description-tabs.component';
import { AsyncPipe } from '@angular/common';
import { switchMap, catchError } from 'rxjs';
import { of } from 'rxjs';

@Component({
  selector: 'app-service-detail-page',
  standalone: true,
  imports: [
    ServiceHeaderComponent,
    ServicePreviewComponent,
    ServicePricingComponent,
    ServiceDescriptionTabsComponent,
    AsyncPipe
  ],
  templateUrl: './service-detail-page.component.html',
  styleUrls: ['./service-detail-page.component.scss']
})
export class ServiceDetailPageComponent {
  private route = inject(ActivatedRoute);
  private serviceService = inject(ServiceDetailService);
  private router = inject(Router);

  service$ = this.route.paramMap.pipe(
    switchMap(params => {
      const id = params.get('id')!;
      return this.serviceService.getService(id).pipe(
        catchError(() => {
          return of(null!);
        })
      );
    })
  );

 onRequestService(serviceId: string) {
    this.router.navigate(['/payment', serviceId]);
  }

onContactProvider() {
  console.log('Contact provider clicked');
}
}



