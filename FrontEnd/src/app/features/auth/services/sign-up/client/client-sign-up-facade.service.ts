import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../../../../../core/interfaces/api-response';
import { ClientSignUpApiService } from './client-sign-up-api.service';
import { ClientSignUpDetails } from '../../../interfaces/sign-up/client/client-sign-up-details';
import { ClientSignUpResponse } from '../../../interfaces/sign-up/client/client-sign-up-response';

@Injectable({
  providedIn: 'root',
})
export class ClientSignUpFacadeService {
  constructor(private api: ClientSignUpApiService) {}

  register(details: ClientSignUpDetails): Observable<ApiResponse<ClientSignUpResponse>> {
    return this.api.register(details);
  }

  public mapError(resOrErr: ApiResponse<any> | HttpErrorResponse | any): { field?: 'phoneNumber' | 'email' | 'global'; message: string } {
    let payload: any = resOrErr;
    if (resOrErr instanceof HttpErrorResponse) {
      payload = resOrErr.error ?? {};
    } else if (resOrErr && resOrErr.error && typeof resOrErr.error === 'object') {
      payload = resOrErr.error;
    }

    const rawMessage = (payload?.message ?? resOrErr?.message ?? '') as string;
    const message = (rawMessage && String(rawMessage).trim()) || 'An unexpected error occurred. Please try again.';
    const lower = message.toLowerCase();

    // Heuristics to attach field-specific errors (no translation)
    if (lower.includes('phone')) {
      return { field: 'phoneNumber', message };
    }
    if (lower.includes('email')) {
      return { field: 'email', message };
    }

    // fallback: global error
    return { field: 'global', message };
  }
}
