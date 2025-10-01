import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Service } from '../interfaces/service.interface';
import { Review } from '../interfaces/service.interface';

@Injectable({
  providedIn: 'root'
})
export class ServiceDetailService {
  private baseUrl = 'https://spacemarket.runasp.net/api/Service/client'; 

  constructor(private http: HttpClient) {}

getService(serviceId: string): Observable<Service> {
  return this.http.get<Service>(`https://spacemarket.runasp.net/api/Service/client/service-detail/${serviceId}`);
}



}



