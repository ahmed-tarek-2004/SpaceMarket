// src/app/features/maps/pages/map-page/map-page.component.ts

import { Component, OnInit, OnDestroy } from '@angular/core';
import { interval, Subscription } from 'rxjs';
import { DebrisAlertService } from '../../services/collision-alert.service';
import { CollisionAlertResponse } from '../../interfaces/collision-alert';
import { MapComponent } from '../../components/map-detail/map.component';

@Component({
  selector: 'app-map-page',
  templateUrl: './map-page.component.html',
  styleUrls: ['./map-page.component.scss'],
  standalone: true,
  imports: [MapComponent]
})
export class MapPageComponent implements OnInit, OnDestroy {
  alerts: CollisionAlertResponse[] = [];
  private pollingSubscription!: Subscription;

  constructor(private debrisAlertService: DebrisAlertService) {}

  ngOnInit(): void {
    this.loadAlerts();
    this.pollingSubscription = interval(10000).subscribe(() => {
      this.loadAlerts();
    });
  }

  loadAlerts(): void {
    this.debrisAlertService.getCollisionAlerts().subscribe({
      next: (data) => {
        this.alerts = data;
      },
      error: (err) => {
        console.error('Failed to load warning data', err);
      }
    });
  }

  ngOnDestroy(): void {
    if (this.pollingSubscription) {
      this.pollingSubscription.unsubscribe();
    }
  }
}