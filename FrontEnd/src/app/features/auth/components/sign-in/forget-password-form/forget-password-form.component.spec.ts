import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ForgetPasswordFormComponent } from './forget-password-form.component';

describe('ForgetPasswordFormComponent', () => {
  let component: ForgetPasswordFormComponent;
  let fixture: ComponentFixture<ForgetPasswordFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ForgetPasswordFormComponent] 
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ForgetPasswordFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});