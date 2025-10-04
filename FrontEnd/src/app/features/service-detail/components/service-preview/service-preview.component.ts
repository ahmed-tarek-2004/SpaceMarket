import { Component, Input } from '@angular/core';
import { ServiceDetailsResponse } from '../../interfaces/service.interface';

@Component({
  selector: 'app-service-preview',
  standalone: true,
  templateUrl: './service-preview.component.html',
  styleUrls: ['./service-preview.component.scss'],
})
export class ServicePreviewComponent {
  @Input() service!: ServiceDetailsResponse;
  activeTab = 'description';

  setTab(tab: string) {
    this.activeTab = tab;
  }
}
