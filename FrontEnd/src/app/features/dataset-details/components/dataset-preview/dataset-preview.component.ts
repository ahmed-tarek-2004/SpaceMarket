import { Component, Input } from "@angular/core";
import { DatasetDetailsResponse } from "../../interfaces/dataset-details-response";

@Component({
  selector: 'app-dataset-preview',
  standalone: true,
  templateUrl: './dataset-preview.component.html',
  styleUrls: ['./dataset-preview.component.scss'],
})
export class DatasetPreviewComponent {
  @Input() dataset!: DatasetDetailsResponse;
  activeTab = 'description';

  setTab(tab: string) {
    this.activeTab = tab;
  }
}
