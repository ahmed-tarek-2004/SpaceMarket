import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs/operators';
import { FormsModule } from '@angular/forms';
import { ServiceMetricsResponse } from '../../interfaces/service/service-metrics-response';
import { ProviderService } from '../../services/provider.service';

@Component({
  selector: 'app-provider-metrics',
  imports: [FormsModule, CommonModule],
  templateUrl: './provider-metrics.component.html',
  styleUrls: ['./provider-metrics.component.scss'],
})
export class ProviderMetricsComponent implements OnInit {
  loading = false;
  metrics: ServiceMetricsResponse[] = [];
  errorMessage: string | null = null;

  dateRange = {
    startDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
    endDate: new Date().toISOString().split('T')[0],
  };

  // Add this property to use in template
  maxDate = new Date().toISOString().split('T')[0];

  constructor(private serviceService: ProviderService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.fetchMetrics();
  }

  fetchMetrics() {
    this.loading = true;
    this.errorMessage = null;
    this.cdr.detectChanges();

    this.serviceService
      .getServiceMetrics(this.dateRange.startDate, this.dateRange.endDate)
      .subscribe({
        next: (response) => {
          if (response.succeeded && response.data) {
            this.metrics = response.data;
          } else {
            this.errorMessage = response.message || 'Failed to fetch metrics';
            this.metrics = [];
          }
          this.loading = false;
          this.cdr.detectChanges(); 
        },
        error: (error) => {
          console.error('Error fetching metrics:', error);
          this.errorMessage = 'An error occurred while fetching metrics. Please try again.';
          this.metrics = [];
          this.loading = false;
          this.cdr.detectChanges();
        },
      });
  }

  getTotalViews(): number {
    return this.metrics.reduce((sum, metric) => sum + metric.viewsCount, 0);
  }

  getTotalClicks(): number {
    return this.metrics.reduce((sum, metric) => sum + metric.clicksCount, 0);
  }

  getTotalRequests(): number {
    return this.metrics.reduce((sum, metric) => sum + metric.requestsCount, 0);
  }

  getConversionRate(metric: ServiceMetricsResponse): number {
    return metric.viewsCount > 0 ? (metric.requestsCount / metric.viewsCount) * 100 : 0;
  }

  resetDateRange() {
    this.dateRange = {
      startDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
      endDate: new Date().toISOString().split('T')[0],
    };
    this.fetchMetrics();
  }
}
