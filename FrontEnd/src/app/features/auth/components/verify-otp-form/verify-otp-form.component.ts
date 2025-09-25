import { Component, inject, ViewChild, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { SignInFacadeService } from '../../services/sign-in/sign-in-facade.service';


@Component({
  selector: 'app-verify-otp-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './verify-otp-form.component.html',
  styleUrls: ['./verify-otp-form.component.scss']
})
export class VerifyOtpFormComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  facade = inject(SignInFacadeService);
  

  otpForm: FormGroup = this.fb.group({
    digit1: [''],
    digit2: [''],
    digit3: [''],
    digit4: [''],
    digit5: [''],
    digit6: ['']
  });

  @ViewChildren('digit1, digit2, digit3, digit4, digit5, digit6') 
  digitInputs!: QueryList<ElementRef<HTMLInputElement>>;

  isLoading = false;
  errorMessage: string | null = null;

  onDigitInput(index: number): void {
    const controlName = `digit${index + 1}`;
    const value = this.otpForm.get(controlName)?.value;

    if (value && /^\d$/.test(value)) {
      if (index < 5) {
        const inputs = this.digitInputs.toArray();
        inputs[index + 1].nativeElement.focus();
      }
    } else {
      this.otpForm.get(controlName)?.setValue('');
    }
  }

  onBackspace(index: number): void {
    const controlName = `digit${index + 1}`;
    const currentValue = this.otpForm.get(controlName)?.value;

    if (!currentValue && index > 0) {
      const inputs = this.digitInputs.toArray();
      inputs[index - 1].nativeElement.focus();
    }
  }

  isOtpComplete(): boolean {
    return Object.values(this.otpForm.value).every(digit => digit !== '');
  }

  onSubmit(): void {
  if (!this.isOtpComplete()) return;
  const otp = Object.values(this.otpForm.value).join('');
  this.facade.verifyOtp(otp).subscribe(result => {
    if (result) this.router.navigate(['/auth/reset-password']);
  });
}
}