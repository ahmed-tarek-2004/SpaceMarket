// src/app/features/service-detail/components/service-description-tabs/service-description-tabs.component.ts
import { Component, Input } from '@angular/core';
import { Review } from '../../interfaces/service.interface';

@Component({
  selector: 'app-service-description-tabs',
  standalone: true,
  templateUrl: './service-description-tabs.component.html',
  styleUrls: ['./service-description-tabs.component.scss']
})
export class ServiceDescriptionTabsComponent {
  @Input() description: string = '';
  @Input() useCases: string[] = [];
  @Input() features: string[] = [];
  @Input() providerInfo: string = '';
  @Input() reviews: Review[] = [];

  activeTab: 'description' | 'provider' | 'reviews' = 'description';

  setTab(tab: 'description' | 'provider' | 'reviews') {
    this.activeTab = tab;
  }
}