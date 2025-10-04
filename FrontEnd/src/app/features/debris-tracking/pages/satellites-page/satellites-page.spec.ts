import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SatellitesPage } from './satellites-page';

describe('SatellitesPage', () => {
  let component: SatellitesPage;
  let fixture: ComponentFixture<SatellitesPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SatellitesPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SatellitesPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
