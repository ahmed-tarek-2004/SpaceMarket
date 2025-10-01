import { Component, inject, ViewChildren, QueryList, ElementRef, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { SignInFacadeService } from '../../services/sign-in/sign-in-facade.service';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { ToastService } from '../../../../shared/services/toast.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-verify-otp-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent],
  templateUrl: './verify-otp-form.component.html',
  styleUrls: ['./verify-otp-form.component.scss'],
})
export class VerifyOtpFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toast = inject(ToastService);
  facade = inject(SignInFacadeService);

  userId: string | null = null;
  email: string | null = null;
  context: string = 'login'; // 'login', 'registration', or 'reset'
  isLoading = false;
  errorMessage: string = '';

  // Create form with individual digit controls
  otpForm: FormGroup = this.fb.group({
    digit1: ['', [Validators.required, Validators.pattern(/^\d$/)]],
    digit2: ['', [Validators.required, Validators.pattern(/^\d$/)]],
    digit3: ['', [Validators.required, Validators.pattern(/^\d$/)]],
    digit4: ['', [Validators.required, Validators.pattern(/^\d$/)]],
    digit5: ['', [Validators.required, Validators.pattern(/^\d$/)]],
    digit6: ['', [Validators.required, Validators.pattern(/^\d$/)]],
  });

  @ViewChildren('digitInput') digitInputs!: QueryList<ElementRef<HTMLInputElement>>;

  ngOnInit() {
    // Get userId, email, and context from query params
    this.route.queryParams.subscribe((params) => {
      this.userId = params['userId'] || this.facade.getCurrentUserId() || null;
      this.email = params['email'] || null;
      this.context = params['context'] || 'login';
      console.log(
        'OTP Component - UserId:',
        this.userId,
        'Email:',
        this.email,
        'Context:',
        this.context
      );

      if (!this.userId) {
        this.toast.error('User ID not found. Please try the process again.');
        this.router.navigate(['/auth/login']);
      }
    });
  }

  onDigitInput(index: number, event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;

    // Only allow digits and set the form control value
    if (!/^\d$/.test(value)) {
      input.value = '';
      this.otpForm.get(`digit${index + 1}`)?.setValue('');
      return;
    }

    // Keep control value synced
    this.otpForm.get(`digit${index + 1}`)?.setValue(value);

    // Auto-focus next input
    if (value && index < 5) {
      const inputs = this.digitInputs.toArray();
      inputs[index + 1].nativeElement.focus();
      inputs[index + 1].nativeElement.select();
    }

    // If all digits are filled, auto-submit
    if (this.isOtpComplete()) {
      this.onSubmit();
    }
  }

  onKeyDown(event: KeyboardEvent, index: number): void {
    // Handle backspace
    if (event.key === 'Backspace') {
      const input = event.target as HTMLInputElement;

      if (!input.value && index > 0) {
        // Move to previous input if current is empty
        const inputs = this.digitInputs.toArray();
        inputs[index - 1].nativeElement.focus();
        inputs[index - 1].nativeElement.select();
      } else if (input.value) {
        // Clear current input and control
        input.value = '';
        this.otpForm.get(`digit${index + 1}`)?.setValue('');
      }
    }

    // Handle arrow keys
    if (event.key === 'ArrowLeft' && index > 0) {
      const inputs = this.digitInputs.toArray();
      inputs[index - 1].nativeElement.focus();
      inputs[index - 1].nativeElement.select();
    } else if (event.key === 'ArrowRight' && index < 5) {
      const inputs = this.digitInputs.toArray();
      inputs[index + 1].nativeElement.focus();
      inputs[index + 1].nativeElement.select();
    }
  }

  onPaste(event: ClipboardEvent): void {
    event.preventDefault();
    const pastedData = event.clipboardData?.getData('text') || '';
    const digits = pastedData.replace(/\D/g, '').slice(0, 6); // Only digits, max 6

    // Fill the form with pasted digits and update visible inputs
    const inputs = this.digitInputs.toArray();
    for (let i = 0; i < digits.length; i++) {
      this.otpForm.get(`digit${i + 1}`)?.setValue(digits[i]);
      if (inputs[i]) inputs[i].nativeElement.value = digits[i];
    }

    // Focus the next empty input or submit if complete
    if (digits.length === 6) {
      this.onSubmit();
    } else if (digits.length < 6 && inputs[digits.length]) {
      inputs[digits.length].nativeElement.focus();
    }
  }

  isOtpComplete(): boolean {
    const values = Object.values(this.otpForm.value);
    return values.every((digit) => digit !== '' && digit !== null);
  }

  getOtpString(): string {
    return [
      this.otpForm.get('digit1')!.value,
      this.otpForm.get('digit2')!.value,
      this.otpForm.get('digit3')!.value,
      this.otpForm.get('digit4')!.value,
      this.otpForm.get('digit5')!.value,
      this.otpForm.get('digit6')!.value,
    ].join('');
  }

  // ADD THIS MISSING METHOD
  clearForm(): void {
    this.otpForm.reset();
    // Clear all input values
    this.digitInputs.forEach((input) => {
      input.nativeElement.value = '';
    });
    // Focus first input
    if (this.digitInputs.first) {
      this.digitInputs.first.nativeElement.focus();
    }
  }

  // ADD THIS MISSING METHOD FOR TEMPLATE
  shouldShowError(controlName: string): boolean {
    const control = this.otpForm.get(controlName);
    return !!(control && control.touched && control.invalid);
  }

  onSubmit(): void {
    if (!this.isOtpComplete()) {
      this.toast.error('Please enter the complete 6-digit OTP');
      return;
    }

    if (!this.userId) {
      this.toast.error('User ID not found. Please try the process again.');
      return;
    }

    const otp = this.getOtpString();
    this.isLoading = true;
    this.errorMessage = '';

    console.log('Submitting OTP:', otp, 'for user:', this.userId, 'context:', this.context);

    if (this.context === 'login') {
      // Verify OTP for login flow
      this.facade
        .verifyOtpForLogin(otp, this.userId)
        .pipe(finalize(() => (this.isLoading = false)))
        .subscribe({
          next: () => this.toast.success('Login successful!'),
          error: (error) => this.handleOtpError(error),
        });
    } else if (this.context === 'reset') {
      // Verify OTP for password reset flow
      this.verifyOtpForReset(otp);
    } else {
      // Verify OTP for registration flow
      this.facade
        .verifyOtpForRegistration(this.userId, otp)
        .pipe(finalize(() => (this.isLoading = false)))
        .subscribe({
          next: () => this.toast.success('Email verified successfully!'),
          error: (error) => this.handleOtpError(error),
        });
    }
  }

  private verifyOtpForReset(otp: string): void {
    // For reset password, we need to verify OTP and then navigate to reset password page
    this.facade
      .verifyOtpForResetPassword(this.userId!, otp)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: () => {
          this.toast.success('OTP verified successfully');
          // Navigate to reset password page with OTP
          this.router.navigate(['/auth/reset-password'], {
            queryParams: {
              userId: this.userId,
              otp: otp, // Pass OTP to reset password page
            },
          });
        },
        error: (error) => this.handleOtpError(error),
      });
  }

  private handleOtpError(error: any): void {
    let errorMessage = 'Invalid OTP. Please try again.';

    if (error?.error && typeof error.error === 'object') {
      const apiError = error.error;
      if (apiError.message) errorMessage = apiError.message;
    } else if (error?.message) {
      errorMessage = error.message;
    }

    this.errorMessage = errorMessage;
    this.toast.error(errorMessage);
    this.clearForm(); // This was causing the error - now it's defined
  }

  resendOtp(): void {
    if (!this.email) {
      this.toast.error('Email not found. Please try again.');
      return;
    }

    this.isLoading = true;

    if (this.context === 'login') {
      // Resend OTP for login
      this.facade
        .login(this.email, this.facade.request.password)
        .pipe(finalize(() => (this.isLoading = false)))
        .subscribe({
          next: () => this.toast.success('OTP resent to your email'),
          error: (error) => this.toast.error('Failed to resend OTP. Please try again.'),
        });
    } else if (this.context === 'reset') {
      // Resend OTP for password reset
      this.facade
        .forgetPassword(this.email)
        .pipe(finalize(() => (this.isLoading = false)))
        .subscribe({
          next: () => this.toast.success('OTP resent to your email'),
          error: (error) => this.toast.error('Failed to resend OTP. Please try again.'),
        });
    } else {
      this.toast.info('Please use the resend option from the registration page');
      this.isLoading = false;
    }
  }

  goBack(): void {
    if (this.context === 'reset') {
      this.router.navigate(['/auth/forgot-password']);
    } else {
      this.router.navigate(['/auth/login']);
    }
  }
}
