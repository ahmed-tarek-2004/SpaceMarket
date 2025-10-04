import { TestBed } from '@angular/core/testing';

import { OrderServiceApi } from './order-service-api';

describe('OrderServiceApi', () => {
  let service: OrderServiceApi;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OrderServiceApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
