import { Component, OnInit } from '@angular/core';
import { ProviderServicesComponent } from "../../components/provider-services/provider-services.component";
import { ProviderDatasetsComponent } from '../../components/provider-datasets/provider-datasets.component';
import { ProviderMetricsComponent } from '../../components/provider-metrics/provider-metrics.component';

@Component({
  selector: 'app-provider-dashboard-page',
  imports: [ProviderServicesComponent, ProviderDatasetsComponent, ProviderMetricsComponent],
  templateUrl: './provider-dashboard-page.component.html',
  styleUrls: ['./provider-dashboard-page.component.scss'],
})
export class ProviderDashboardPageComponent {
  activeTab = 'services';

  tabs = [
    { id: 'services', name: 'My Services' },
    { id: 'datasets', name: 'My Datasets' },
    { id: 'metrics', name: 'Metrics' },
  ];

  setActiveTab(tabId: string) {
    this.activeTab = tabId;
  }

  getTabClasses(tabId: string): string {
    const baseClasses = 'px-6 py-3 rounded-xl font-medium transition-all duration-300';
    if (this.activeTab === tabId) {
      return `${baseClasses} bg-gradient-to-r from-[#00A8FF] to-[#0077FF] text-white`;
    }
    return `${baseClasses} bg-white/5 text-gray-300 hover:bg-white/10 hover:text-white border border-[#00A8FF]/20`;
  }
}
