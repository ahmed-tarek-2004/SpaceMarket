/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { ProviderMetricsComponent } from './provider-metrics.component';

describe('ProviderMetricsComponent', () => {
  let component: ProviderMetricsComponent;
  let fixture: ComponentFixture<ProviderMetricsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProviderMetricsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProviderMetricsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
