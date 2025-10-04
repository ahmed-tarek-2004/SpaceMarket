import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { SatelliteListComponent } from '../../components/satellite-list/satellite-list.component';
import { AllSatellitesComponent } from '../../components/all-satellites/all-satellites.component';
import { DebrisApiServiceService } from '../../services/debris-api-service.service';
import { Satellite } from '../../interfaces/satellite';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-satellites-page',
  standalone: true,
  imports: [CommonModule, SatelliteListComponent, AllSatellitesComponent],
  templateUrl: './satellites-page.html',
  styleUrl: './satellites-page.scss',
})
export class SatellitesPage implements OnInit {
  private debrisApiService = inject(DebrisApiServiceService);
  private toastService = inject(ToastService);
  private router = inject(Router);

  satellites: Satellite[] = [];
  isLoadingSatellites = signal(false);
  hasSatelliteError = signal(false);
  satelliteErrorMessage = signal('');

  ngOnInit(): void {
    this.loadSatellites();
  }

  loadSatellites(): void {
    this.isLoadingSatellites.set(true);
    this.hasSatelliteError.set(false);
    this.satelliteErrorMessage.set('');

    this.debrisApiService.getMySatellites().subscribe({
      next: (response) => {
        this.isLoadingSatellites.set(false);
        if (response.succeeded) {
          this.satellites = response.data || [];
        } else {
          this.hasSatelliteError.set(true);
          this.satelliteErrorMessage.set(response.message || 'Failed to load satellites');
        }
      },
      error: (error) => {
        this.isLoadingSatellites.set(false);
        this.hasSatelliteError.set(true);
        console.error('Error loading satellites:', error);

        if (error.error?.message) {
          this.satelliteErrorMessage.set(error.error.message);
        } else if (error.message) {
          this.satelliteErrorMessage.set(error.message);
        } else {
          this.satelliteErrorMessage.set('Failed to load satellites. Please try again later.');
        }
      },
    });
  }

  onSatelliteSelected(satellite: { id: string; name: string; noradId: string }): void {
    console.log('Satellite selected:', satellite);
    // Navigate to registration page with satellite data as query parameters
    this.router.navigate(['/satellite-registration'], {
      queryParams: {
        id: satellite.id,
        name: satellite.name,
        noradId: satellite.noradId,
      },
    });
  }

  refreshSatellites(): void {
    this.loadSatellites();
  }

  onSatelliteClicked(satellite: { id: string; name: string }): void {
    console.log('Satellite clicked in satellites page:', satellite);
    // Navigation is handled in the satellite-list component
  }
}
