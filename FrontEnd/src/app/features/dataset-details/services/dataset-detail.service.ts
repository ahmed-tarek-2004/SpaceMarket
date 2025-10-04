import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { DatasetDetailsResponse } from '../interfaces/dataset-details-response';
import { map, Observable } from 'rxjs';
import { ApiResponse } from '../../../core/interfaces/api-response';

@Injectable({
  providedIn: 'root'
})
export class DatasetDetailService {
  private baseUrl = `${environment.apiUrl}${environment.service.datasetDetails}`;

  constructor(private http: HttpClient) {}

  getDataset(datasetId: string): Observable<DatasetDetailsResponse> {
    return this.http
      .get<ApiResponse<DatasetDetailsResponse>>(`${this.baseUrl}${datasetId}`)
      .pipe(map((response) => response.data));
  }
}
