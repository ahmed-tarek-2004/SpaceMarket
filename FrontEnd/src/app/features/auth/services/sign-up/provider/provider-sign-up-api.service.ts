import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { ApiResponse } from '../../../../../core/interfaces/api-response';
import { ProviderSignUpDetails } from '../../../interfaces/sign-up/provider/provider-sign-up-details';
import { ProviderSignUpResponse } from '../../../interfaces/sign-up/provider/provider-sign-up-response';

@Injectable({
  providedIn: 'root',
})
export class ProviderSignUpApiService {
  private readonly _apiUrl = `${environment.apiUrl}${environment.account.providerSignup}`;

  constructor(private http: HttpClient) {}

  register(
    details: ProviderSignUpDetails & { CertificationFiles?: File[] }
  ): Observable<ApiResponse<ProviderSignUpResponse>> {
    const formData = new FormData();

    formData.append('Email', details.Email);
    formData.append('PhoneNumber', details.PhoneNumber);
    formData.append('Password', details.Password);
    formData.append('CompanyName', details.CompanyName);
    formData.append('WebsiteUrl', details.WebsiteUrl);

    if (details.CertificationFiles) {
      details.CertificationFiles.forEach((file) => {
        formData.append('CertificationFiles', file, file.name);
      });
    }

    return this.http.post<ApiResponse<ProviderSignUpResponse>>(this._apiUrl, formData);
  }
}
