import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CreateServiceRequest } from '../interfaces/create-service-request';

@Injectable({
  providedIn: 'root',
})
export class CreateServiceApiService {
  private baseUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  createService(createServiceRequest: CreateServiceRequest): Observable<any> {
    const formData = new FormData();
    formData.append('Title', createServiceRequest.Title);
    formData.append('Description', createServiceRequest.Description);
    formData.append('CategoryId', createServiceRequest.CategoryId);
    formData.append('Price', createServiceRequest.Price.toString());
    formData.append('Image', createServiceRequest.Image, createServiceRequest.Image.name);

    return this.http.post(`${this.baseUrl}${environment.service.createService}`, formData);
  }
}
