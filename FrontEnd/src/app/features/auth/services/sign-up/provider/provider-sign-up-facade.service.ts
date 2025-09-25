import { Injectable } from '@angular/core';
import { ProviderSignUpApiService } from './provider-sign-up-api.service';
import { ProviderSignUpDetails } from '../../../interfaces/sign-up/provider/provider-sign-up-details';

@Injectable({ providedIn: 'root' })
export class ProviderSignUpFacadeService {
  constructor(private api: ProviderSignUpApiService) {}

  register(details: ProviderSignUpDetails & { CertificationFiles?: File[] }) {
    return this.api.register(details);
  }

  mapError(error: any): { field?: 'phoneNumber' | 'email' | 'global'; message: string } {
    // try to extract backend message string
    const raw = (error?.error?.message ?? error?.message ?? '').toString().trim();
    const msg = raw || 'An unexpected error occurred. Please try again.';

    const lower = msg.toLowerCase();

    // Field-specific heuristics (add more rules as needed)
    if (lower.includes('phone')) {
      return { field: 'phoneNumber', message: msg };
    }
    if (lower.includes('email')) {
      return { field: 'email', message: msg };
    }

    // fallback to global
    return { field: 'global', message: msg };
  }
}
