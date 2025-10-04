import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { SatelliteFormComponent } from '../../components/satellite-form/satellite-form.component';
import { DebrisApiServiceService } from '../../services/debris-api-service.service';
import { RegisterSatelliteRequest } from '../../interfaces/register-satellite-request';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-satellite-registration-page',
  standalone: true,
  imports: [CommonModule, SatelliteFormComponent],
  templateUrl: './satellite-registration-page.html',
  styleUrl: './satellite-registration-page.scss',
})
export class SatelliteRegistrationPage implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private debrisApiService = inject(DebrisApiServiceService);
  private toastService = inject(ToastService);

  isSubmitting = signal(false);
  selectedSatellite: { id: string; name: string; noradId: string } | null = null;

  ngOnInit(): void {
    // Get satellite data from route parameters
    this.route.queryParams.subscribe((params) => {
      if (params['id'] && params['name'] && params['noradId']) {
        this.selectedSatellite = {
          id: params['id'],
          name: params['name'],
          noradId: params['noradId'],
        };
      } else {
        // If no satellite selected, redirect back to satellites page
        this.router.navigate(['/client-dashboard']);
      }
    });
  }

  onFormSubmit(formData: RegisterSatelliteRequest) {
    console.log('Form submission received in satellite registration page');
    console.log('Form data:', formData);
    console.log('Selected satellite:', this.selectedSatellite);
    console.log('Is submitting:', this.isSubmitting());

    if (this.isSubmitting()) {
      console.log('Already submitting, ignoring request');
      return;
    }

    this.isSubmitting.set(true);

    // Use the selected satellite data for registration
    const registrationData: RegisterSatelliteRequest = {
      catalogSatelliteId: this.selectedSatellite?.id || formData.catalogSatelliteId,
      name: this.selectedSatellite?.name || formData.name,
      proximityThresholdKm: formData.proximityThresholdKm,
    };

    console.log('Registration data:', registrationData);

    this.debrisApiService.registerSatellite(registrationData).subscribe({
      next: (response) => {
        this.isSubmitting.set(false);
        if (response.succeeded) {
          this.toastService.success(
            `Satellite "${registrationData.name}" has been successfully registered for debris tracking!`,
            6000
          );
          // Navigate back to satellites page
          this.router.navigate(['/client-dashboard']);
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
}
