import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SatelliteFormComponent } from '../../components/satellite-form/satellite-form.component';
import { SatelliteListComponent } from '../../components/satellite-list/satellite-list.component';
import { DebrisApiServiceService } from '../../services/debris-api-service.service';
import { RegisterSatelliteRequest } from '../../interfaces/register-satellite-request';
import { Satellite } from '../../interfaces/satellite';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-satellites-page',
  standalone: true,
  imports: [CommonModule, SatelliteFormComponent, SatelliteListComponent],
  templateUrl: './satellites-page.html',
  styleUrl: './satellites-page.scss',
})
export class SatellitesPage implements OnInit {
  private debrisApiService = inject(DebrisApiServiceService);
  private toastService = inject(ToastService);

  isSubmitting = signal(false);
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

  onFormSubmit(formData: RegisterSatelliteRequest) {
    if (this.isSubmitting()) return;

    this.isSubmitting.set(true);

    this.debrisApiService.registerSatellite(formData).subscribe({
      next: (response) => {
        this.isSubmitting.set(false);
        if (response.succeeded) {
          this.toastService.success(
            `Satellite "${formData.name}" has been successfully registered for debris tracking!`,
            6000
          );
          // Refresh the satellite list
          this.loadSatellites();
        } else {
          this.toastService.error(
            response.message || 'Failed to register satellite. Please try again.',
            6000
          );
        }
      },
      error: (error) => {
        this.isSubmitting.set(false);
        console.error('Error registering satellite:', error);

        let errorMessage = 'Failed to register satellite. Please try again.';

        if (error.error?.message) {
          errorMessage = error.error.message;
        } else if (error.message) {
          errorMessage = error.message;
        }

        this.toastService.error(errorMessage, 6000);
      },
    });
  }

  refreshSatellites(): void {
    this.loadSatellites();
  }
}
