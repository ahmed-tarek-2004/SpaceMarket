import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-service-preview',
  standalone: true,
  templateUrl: './service-preview.component.html',
  styleUrls: ['./service-preview.component.scss']
})
export class ServicePreviewComponent {
  @Input() images: string[] = [];
  selectedIndex = 0;

  selectImage(index: number) {
    this.selectedIndex = index;
  }
}
