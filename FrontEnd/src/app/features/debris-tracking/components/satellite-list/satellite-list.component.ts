import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Satellite } from '../../interfaces/satellite';

@Component({
  selector: 'app-satellite-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './satellite-list.component.html',
  styleUrls: ['./satellite-list.component.scss'],
})
export class SatelliteListComponent {
  @Input() satellites: Satellite[] = [];
  @Input() isLoading = false;
  @Input() hasError = false;
  @Input() errorMessage = '';
  @Output() satelliteClicked = new EventEmitter<{ id: string; name: string }>();

  constructor(private router: Router) {}

  getStatusColor(threshold: number): string {
    if (threshold <= 5) return 'text-red-400';
    if (threshold <= 10) return 'text-yellow-400';
    return 'text-green-400';
  }

  getStatusText(threshold: number): string {
    if (threshold <= 5) return 'High Risk';
    if (threshold <= 10) return 'Medium Risk';
    return 'Low Risk';
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  getRelativeTime(date: Date): string {
    const now = new Date();
    const diffInHours = Math.floor((now.getTime() - new Date(date).getTime()) / (1000 * 60 * 60));

    if (diffInHours < 24) {
      return `${diffInHours}h ago`;
    }

    const diffInDays = Math.floor(diffInHours / 24);
    if (diffInDays < 7) {
      return `${diffInDays}d ago`;
    }

    return this.formatDate(date);
  }

  trackBySatelliteId(index: number, satellite: Satellite): string {
    return satellite.id;
  }

  onSatelliteClick(satellite: Satellite): void {
    console.log('Satellite clicked:', satellite);
    this.satelliteClicked.emit({
      id: satellite.id,
      name: satellite.name,
    });

    // Navigate to map with satellite ID as route parameter
    this.router.navigate(['/maps', satellite.id], {
      queryParams: {
        satelliteName: satellite.name,
      },
    });
  }
}
