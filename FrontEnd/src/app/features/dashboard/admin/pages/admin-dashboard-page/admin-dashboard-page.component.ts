import { Component, AfterViewInit } from '@angular/core';
import { CartContentComponent } from '../../components/cart-content/cart-content.component';
import { ServiceListAdminComponent } from '../../components/service-list-admin/service-list-admin.component';
import { DatasetListAdminComponent } from '../../components/dataset-list-admin/dataset-list-admin.component';
import { SpaceThemeService } from '../../../../../shared/services/space-theme.service';

@Component({
  selector: 'app-admin-dashboard-page',
  templateUrl: './admin-dashboard-page.component.html',
  styleUrls: ['./admin-dashboard-page.component.scss'],
  imports: [ ServiceListAdminComponent, DatasetListAdminComponent],
})
export class AdminDashboardPageComponent implements AfterViewInit {
  activeTab = 'service-list';

  tabs = [
    { id: 'service-list', name: 'Service List' },
    { id: 'dataset-list', name: 'Dataset List' },
  ];

  constructor(private spaceThemeService: SpaceThemeService) {}

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.spaceThemeService.initializeStarField('star-field');
    }, 100);
  }

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
