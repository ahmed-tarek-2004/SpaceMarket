import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentButton } from './payment-button';

describe('PaymentButton', () => {
  let component: PaymentButton;
  let fixture: ComponentFixture<PaymentButton>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaymentButton]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaymentButton);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
