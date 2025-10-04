import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CreateDatasetRequest } from '../interfaces/create-dataset-request';

@Injectable({
  providedIn: 'root',
})
export class CreateDatasetApiService {
  private baseUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  createDataset(
    createDatasetRequest: CreateDatasetRequest,
    file: File,
    thumbnail?: File
  ): Observable<any> {
    const formData = new FormData();

    // Append all properties from the request
    formData.append('Title', createDatasetRequest.title);
    formData.append('Description', createDatasetRequest.description || '');
    formData.append('CategoryId', createDatasetRequest.categoryId);
    formData.append('Price', createDatasetRequest.price.toString());

    if (createDatasetRequest.apiEndpoint) {
      formData.append('ApiEndpoint', createDatasetRequest.apiEndpoint);
    }

    // Append files
    formData.append('File', file);

    if (thumbnail) {
      formData.append('Thumbnail', thumbnail);
    }

    return this.http.post(`${this.baseUrl}${environment.service.createDataset}`, formData);
  }
}
