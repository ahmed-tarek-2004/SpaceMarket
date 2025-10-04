import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { SignInFacadeService } from '../../../services/sign-in/sign-in-facade.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { finalize } from 'rxjs';
import { ButtonComponent } from '../../../../../shared/components/button/button.component';
import { InputComponent } from '../../../../../shared/components/input/input.component'; // Add this import

@Component({
  selector: 'app-forget-password-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonComponent,
    InputComponent, // Add this to imports array
  ],
  templateUrl: './forget-password-form.component.html',
  styleUrls: ['./forget-password-form.component.scss'],
})
export class ForgetPasswordFormComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private toast = inject(ToastService);
  facade = inject(SignInFacadeService);

  emailForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
  });

  isLoading = false;
  errorMessage: string | null = null;

  get email() {
    return this.emailForm.get('email');
  }

  // Check if field should show validation error (only for styling)
  shouldShowError(controlName: string): boolean {
    const control = this.emailForm.get(controlName);
    return !!(control && control.touched && control.invalid);
  }

  // Get validation message for template
  getValidationMessage(controlName: string): string | null {
    const control = this.emailForm.get(controlName);
    if (!control || !control.touched) return null;

    if (control.hasError('required')) return 'Email is required';
    if (control.hasError('email')) return 'Please enter a valid email address';

    return null;
  }

  goBack() {
    this.router.navigate(['/auth/sign-in']);
  }

  onSubmit(): void {
    // Mark all fields as touched to trigger validation display
    this.emailForm.markAllAsTouched();

    if (this.emailForm.invalid) {
      // Show toast for form validation errors
      if (this.email?.errors?.['required']) {
        this.toast.error('Email is required');
      } else if (this.email?.errors?.['email']) {
        this.toast.error('Please enter a valid email address');
      } else {
        this.toast.error('Please check your form for errors');
      }
      return;
    }

    const email = this.emailForm.value.email;
    this.isLoading = true;
    this.errorMessage = null;

    this.facade
      .forgetPassword(email)
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe({
        next: (response) => {
          if (response !== null) {
            this.toast.success('OTP sent to your email');
            // Navigate to OTP page with email only (no userId for forget password flow)
            this.router.navigate(['/auth/verify-otp'], {
              queryParams: {
                email: email,
                context: 'reset', // Important: set context to reset for password reset flow
              },
            });
          }
        },
        error: (error) => {
          let errorMessage = 'Failed to send reset instructions';

          if (error?.error && typeof error.error === 'object') {
            const apiError = error.error;
            if (apiError.message) errorMessage = apiError.message;
          } else if (error?.message) {
            errorMessage = error.message;
          }

          this.errorMessage = errorMessage;
          this.toast.error(errorMessage);
        },
      });
  }
}
