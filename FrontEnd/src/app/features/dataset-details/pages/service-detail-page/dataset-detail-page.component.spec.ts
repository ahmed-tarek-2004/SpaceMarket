/* tslint:disable:no-unused-variable */
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { DatasetDetailPageComponent } from './dataset-detail-page.component';

describe('ServiceDetailPageComponent', () => {
  let component: DatasetDetailPageComponent;
  let fixture: ComponentFixture<DatasetDetailPageComponent>;

  beforeEach((() => {
    TestBed.configureTestingModule({
      declarations: [ DatasetDetailPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DatasetDetailPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
