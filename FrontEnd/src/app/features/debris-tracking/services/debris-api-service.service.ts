import { inject, Injectable } from '@angular/core';
import { RegisterSatelliteRequest } from '../interfaces/register-satellite-request';
import { ApiResponse } from '../../../core/interfaces/api-response';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { DebrisAlertHistoryResponse } from '../interfaces/debris-alert-history-response';
import { Satellite } from '../interfaces/satellite';

@Injectable({
  providedIn: 'root',
})
export class DebrisApiServiceService {
  private apiUrl = `${environment.apiUrl}`;
  private http = inject(HttpClient);

  registerSatellite(
    registerSatelliteRequest: RegisterSatelliteRequest
  ): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(
      `${this.apiUrl}${environment.debrisTracking.registerSatellite}`,
      registerSatelliteRequest
    );
  }

  editThreshold(
    satelliteId: string,
    proximityThresholdKm: number
  ): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>(
      `${this.apiUrl}${environment.debrisTracking.editThreshold(satelliteId)}`,
      proximityThresholdKm
    );
  }

  getDebrisAlertHistory(): Observable<ApiResponse<DebrisAlertHistoryResponse[]>> {
    return this.http.get<ApiResponse<DebrisAlertHistoryResponse[]>>(
      `${this.apiUrl}${environment.debrisTracking.debrisAlertHistory}`
    );
  }

  debrisCheck(): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(
      `${this.apiUrl}${environment.debrisTracking.debrisCheck}`,
      {}
    );
  }

  getMySatellites(): Observable<ApiResponse<Satellite[]>> {
    return this.http.get<ApiResponse<Satellite[]>>(
      `${this.apiUrl}${environment.debrisTracking.mySatellites}`
    );
  }
}
