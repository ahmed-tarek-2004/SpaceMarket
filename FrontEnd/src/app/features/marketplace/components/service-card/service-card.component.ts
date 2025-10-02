import { Component, Input } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { ApiServiceItem } from '../../interfaces/api-service-item';
import { Router } from '@angular/router';

@Component({
  selector: 'app-service-card',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './service-card.component.html',
  styleUrls: ['./service-card.component.scss'],
})
export class ServiceCardComponent {
  /** Accept the exact backend model */
  @Input() service!: ApiServiceItem;

  constructor(private router: Router) {}

  viewDetails(): void {
    this.router.navigate(['/service', this.service.id]);
  }

  // Add the missing imageUrl getter
  get imageUrl(): string {
    // If the service has an imagesUrl from the backend, use it
    if (this.service.imagesUrl) {
      return this.service.imagesUrl;
    }

    // Fallback to Unsplash placeholder if no image from backend
    const query = encodeURIComponent(
      (this.service.categoryName ?? this.service.title ?? 'service') + ',satellite'
    );
    return `https://source.unsplash.com/featured/?${query}`;
  }

  handleImageError(event: any) {
    // If the backend image fails to load, fallback to Unsplash
    const target = event.target as HTMLImageElement;
    if (this.service.imagesUrl && target.src !== this.getFallbackImage()) {
      target.src = this.getFallbackImage();
    }
  }

  private getFallbackImage(): string {
    const query = encodeURIComponent(
      (this.service.categoryName ?? this.service.title ?? 'service') + ',satellite'
    );
    return `https://source.unsplash.com/featured/?${query}`;
  }

  get providerInitial(): string {
    return this.service.providerName && this.service.providerName.length
      ? this.service.providerName.charAt(0).toUpperCase()
      : this.service.title
      ? this.service.title.charAt(0).toUpperCase()
      : 'S';
  }

  getStars(rating: number): { filled: boolean }[] {
    const stars = [];
    for (let i = 1; i <= 5; i++) {
      stars.push({ filled: i <= Math.floor(rating) });
    }
    return stars;
  }
}
