import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiDatasetItem } from '../../interfaces/api-dataset-item';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dataset-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dataset-card.component.html',
  styleUrls: ['./dataset-card.component.scss'],
})
export class DatasetCardComponent {
  @Input() dataset!: ApiDatasetItem;

  constructor(private router: Router) {}

  viewDetails(): void {
    this.router.navigate(['/dataset', this.dataset.id]);
  }

  get providerInitial(): string {
    return this.dataset?.providerName?.charAt(0).toUpperCase() ?? '?';
  }

  get imageUrl(): string {
    return this.dataset?.thumbnailUrl ?? 'assets/images/dataset-placeholder.jpg';
  }
}
