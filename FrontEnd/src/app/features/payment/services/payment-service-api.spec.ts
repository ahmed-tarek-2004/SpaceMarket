import { TestBed } from '@angular/core/testing';

import { PaymentServiceApi } from './payment-service-api';

describe('PaymentServiceApi', () => {
  let service: PaymentServiceApi;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PaymentServiceApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
