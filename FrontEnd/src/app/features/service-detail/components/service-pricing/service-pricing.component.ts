// src/app/features/service-detail/components/service-pricing/service-pricing.component.ts
import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-service-pricing',
  standalone: true,
  templateUrl: './service-pricing.component.html',
  styleUrls: ['./service-pricing.component.scss']
})
export class ServicePricingComponent {
  @Input() price: number = 0;
  @Input() currency: string = 'USD';
  @Input() whatsIncluded: string[] = [];
  @Input() serviceId: string = ''; 

  @Output() requestService = new EventEmitter<void>();
  @Output() contactProvider = new EventEmitter<void>();
}