import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { ApiResponse } from '../../../../../core/interfaces/api-response';
import { ClientSignUpDetails } from '../../../interfaces/sign-up/client/client-sign-up-details';
import { ClientSignUpResponse } from '../../../interfaces/sign-up/client/client-sign-up-response';

@Injectable({
  providedIn: 'root',
})
export class ClientSignUpApiService {
  private readonly _apiUrl = `${environment.apiUrl}${environment.account.clientSignup}`;

  constructor(private http: HttpClient) {}

  register(details: ClientSignUpDetails): Observable<ApiResponse<ClientSignUpResponse>> {
    const form = new FormData();
    form.append('FullName', details.FullName ?? '');
    form.append('OrganizationName', details.OrganizationName ?? '');
    form.append('Email', details.Email ?? '');
    form.append('PhoneNumber', details.PhoneNumber ?? '');
    form.append('Password', details.Password ?? '');
    form.append('ConfirmPassword', details.ConfirmPassword ?? '');
    form.append('Country', details.Country ?? '');

    // Do NOT set Content-Type header manually. Browser will set multipart/form-data with boundary.
    return this.http.post<ApiResponse<ClientSignUpResponse>>(this._apiUrl, form);
  }
}
