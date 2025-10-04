// src/app/features/maps/pages/map-page/map-page.component.ts

import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { interval, Subscription } from 'rxjs';
import { DebrisAlertService } from '../../services/collision-alert.service';
import { CollisionAlertResponse, PositionDto } from '../../interfaces/collision-alert';
import { MapComponent } from '../../components/map-detail/map.component';

@Component({
  selector: 'app-map-page',
  templateUrl: './map-page.component.html',
  styleUrls: ['./map-page.component.scss'],
  standalone: true,
  imports: [MapComponent],
})
export class MapPageComponent implements OnInit, OnDestroy {
  @ViewChild(MapComponent) mapComponent!: MapComponent;

  alerts: CollisionAlertResponse[] = [];
  satellitePosition: PositionDto | null = null;
  satelliteId: string | null = null;
  satelliteName: string | null = null;
  private pollingSubscription!: Subscription;

  constructor(private debrisAlertService: DebrisAlertService, private route: ActivatedRoute) {}

  ngOnInit(): void {
    // Get satellite ID from route parameters
    this.route.params.subscribe((params) => {
      this.satelliteId = params['satelliteId'] || null;

      if (this.satelliteId) {
        console.log('Loading satellite position for:', this.satelliteId);
        this.loadSatellitePosition();
      }
    });

    // Get satellite name from query parameters
    this.route.queryParams.subscribe((params) => {
      this.satelliteName = params['satelliteName'] || null;
    });

    this.loadAlerts();
    this.pollingSubscription = interval(10000).subscribe(() => {
      this.loadAlerts();
    });

    // Expose debug methods to window for console access
    (window as any).debugSatellitePosition = () => this.debugSatellitePosition();
    (window as any).reloadSatellitePosition = () => this.reloadSatellitePosition();
    console.log(
      'Debug methods exposed to window: debugSatellitePosition(), reloadSatellitePosition()'
    );
  }

  loadAlerts(): void {
    this.debrisAlertService.getCollisionAlerts().subscribe({
      next: (data) => {
        this.alerts = data;
      },
      error: (err) => {
        console.error('Failed to load warning data', err);
      },
    });
  }

  loadSatellitePosition(): void {
    if (!this.satelliteId) {
      console.log('No satellite ID provided for position loading');
      return;
    }

    console.log('Loading satellite position for ID:', this.satelliteId);
    this.debrisAlertService.getSatellitePosition(this.satelliteId).subscribe({
      next: (data) => {
        console.log('Raw API response:', data);

        // Check if the response has the expected structure
        if (!data || typeof data !== 'object') {
          console.error('Invalid API response format:', data);
          return;
        }

        // Validate required fields
        if (
          typeof data.longitude !== 'number' ||
          typeof data.latitude !== 'number' ||
          typeof data.altitudeKm !== 'number'
        ) {
          console.error('API response missing required fields:', data);
          console.error('Expected: longitude, latitude, altitudeKm as numbers');
          return;
        }

        console.log('Satellite position loaded successfully:', data);
        this.satellitePosition = data;
        console.log('Satellite position set to:', this.satellitePosition);

        // Wait for the map component to be available and then trigger drawing
        this.waitForMapComponentAndDraw();
      },
      error: (err) => {
        console.error('Failed to load satellite position', err);
        console.error('Error details:', err);
        console.error('Error status:', err.status);
        console.error('Error message:', err.message);
      },
    });
  }

  private waitForMapComponentAndDraw(): void {
    const checkMapComponent = () => {
      if (this.mapComponent) {
        console.log('Map component is available, triggering satellite position drawing');

        // Log debug info before attempting to draw
        console.log('Map component debug info:', this.mapComponent.getDebugInfo());

        // Try multiple approaches to ensure the satellite position is drawn
        this.mapComponent.drawSatellitePositionManually();

        // Check if position was displayed after a short delay
        setTimeout(() => {
          if (this.mapComponent) {
            console.log(
              'Checking if satellite position was displayed:',
              this.mapComponent.isSatellitePositionDisplayed()
            );
            console.log('Updated debug info:', this.mapComponent.getDebugInfo());

            // If not displayed, try force redraw
            if (!this.mapComponent.isSatellitePositionDisplayed()) {
              console.log('Satellite position not displayed, trying force redraw');
              this.mapComponent.forceRedrawSatellitePosition();
            }
          }
        }, 1000);

        // Also try force redraw as a final fallback
        setTimeout(() => {
          if (this.mapComponent) {
            this.mapComponent.forceRedrawSatellitePosition();
          }
        }, 3000);
      } else {
        console.log('Map component not yet available, retrying...');
        setTimeout(checkMapComponent, 100);
      }
    };

    checkMapComponent();
  }

  ngOnDestroy(): void {
    if (this.pollingSubscription) {
      this.pollingSubscription.unsubscribe();
    }
  }

  // Method for debugging - can be called from browser console
  public debugSatellitePosition(): void {
    console.log('=== SATELLITE POSITION DEBUG ===');
    console.log('Satellite ID:', this.satelliteId);
    console.log('Satellite Name:', this.satelliteName);
    console.log('Satellite Position:', this.satellitePosition);
    console.log('Map Component Available:', !!this.mapComponent);

    if (this.mapComponent) {
      console.log('Map Component Debug Info:', this.mapComponent.getDebugInfo());
      console.log(
        'Is Satellite Position Displayed:',
        this.mapComponent.isSatellitePositionDisplayed()
      );
    }

    console.log('=== END DEBUG ===');
  }

  // Method to manually reload satellite position
  public reloadSatellitePosition(): void {
    console.log('Manually reloading satellite position...');
    if (this.satelliteId) {
      this.loadSatellitePosition();
    } else {
      console.log('No satellite ID available for reload');
    }
  }
}
