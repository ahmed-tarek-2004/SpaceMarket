// src/app/features/maps/services/debris-alert.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { CollisionAlertResponse, PositionDto } from '../interfaces/collision-alert';

@Injectable({ providedIn: 'root' })
export class DebrisAlertService {
  private apiUrl = 'https://spacemarket.runasp.net/api/DebrisAlert';

  constructor(private http: HttpClient) {}

  // history of alerts
  getCollisionAlerts(): Observable<CollisionAlertResponse[]> {
    return this.http
      .get<{ data: CollisionAlertResponse[] }>(`${this.apiUrl}/alerts/history`)
      .pipe(map((res) => res.data));
  }

  // register new satellite
  registerSatellite(payload: {
    catalogSatelliteId: string;
    name: string;
    proximityThresholdKm: number;
  }): Observable<any> {
    return this.http
      .post<{ data: any }>(`${this.apiUrl}/register-satellite`, payload)
      .pipe(map((res) => res.data));
  }

  // update threshold
  updateThreshold(satelliteId: string, threshold: number): Observable<any> {
    return this.http
      .put<{ data: any }>(`${this.apiUrl}/${satelliteId}/threshold`, {
        proximityThresholdKm: threshold,
      })
      .pipe(map((res) => res.data));
  }

  // get user’s satellites
  getMySatellites(): Observable<any[]> {
    return this.http
      .get<{ data: any[] }>(`${this.apiUrl}/my-satellites`)
      .pipe(map((res) => res.data));
  }

  // check collisions
  checkNow(): Observable<any> {
    return this.http.post<{ data: any }>(`${this.apiUrl}/check`, {}).pipe(map((res) => res.data));
  }

  // get catalog names
  getCatalogNames(): Observable<any[]> {
    return this.http
      .get<{ data: any[] }>(`${this.apiUrl}/catalog/names`)
      .pipe(map((res) => res.data));
  }

  // get satellite position
  getSatellitePosition(satelliteId: string): Observable<PositionDto> {
    return this.http
      .get<{ data: PositionDto }>(`${this.apiUrl}/${satelliteId}/position`)
      .pipe(map((res) => res.data));
  }

  // full catalog
  getCatalog(query: {
    Name?: string;
    NoradId?: string;
    TleLine1?: string;
    TleLine2?: string;
    PageNumber?: number;
    PageSize?: number;
  }): Observable<any> {
    return this.http
      .get<{ data: any }>(`${this.apiUrl}/satelitecatalog`, { params: query as any })
      .pipe(map((res) => res.data));
  }
}
