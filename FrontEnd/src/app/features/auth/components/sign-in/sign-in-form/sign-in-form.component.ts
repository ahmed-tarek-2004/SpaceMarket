import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { SignInFacadeService } from '../../../services/sign-in/sign-in-facade.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { InputComponent } from '../../../../../shared/components/input/input.component';
import { ButtonComponent } from '../../../../../shared/components/button/button.component';
import { ToastService } from '../../../../../shared/services/toast.service';
import { finalize } from 'rxjs';
import { TokenService } from '../../../../../core/services/token.service';

@Component({
  selector: 'app-sign-in-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InputComponent, ButtonComponent],
  templateUrl: './sign-in-form.component.html',
  styleUrls: ['./sign-in-form.component.scss'],
})
export class SignInFormComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private toast = inject(ToastService);
  private tokenService = inject(TokenService)
  facade = inject(SignInFacadeService);

  signInForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  ngOnInit(): void {
    const token = this.tokenService.getAccessToken();
    if (token) {
      // Already signed in → go home
      this.router.navigate(['/']);
    }
  }

  get email() {
    return this.signInForm.get('email');
  }

  get password() {
    return this.signInForm.get('password');
  }

  // Check if field should show validation error (only for styling)
  shouldShowError(controlName: string): boolean {
    const control = this.signInForm.get(controlName);
    return !!(control && control.touched && control.invalid);
  }

  // Get validation message for template (optional - if you still want to show field-level messages)
  getValidationMessage(controlName: string): string | null {
    const control = this.signInForm.get(controlName);
    if (!control || !control.touched) return null;

    if (control.hasError('required')) return 'This field is required';
    if (control.hasError('email')) return 'Please enter a valid email address';
    if (control.hasError('minlength')) {
      const requiredLength = control.errors!['minlength']?.requiredLength ?? 6;
      return `Password must be at least ${requiredLength} characters`;
    }
    return null;
  }

  goToForgotPassword() {
    this.router.navigate(['/auth/forgot-password']);
  }

  onSubmit(): void {
    // Mark all fields as touched to trigger validation display
    this.signInForm.markAllAsTouched();

    if (this.signInForm.invalid) {
      // Show toast for form validation errors
      if (this.email?.errors?.['required'] || this.password?.errors?.['required']) {
        this.toast.error('Please fill in all required fields');
      } else if (this.email?.errors?.['email']) {
        this.toast.error('Please enter a valid email address');
      } else if (this.password?.errors?.['minlength']) {
        this.toast.error('Password must be at least 6 characters long');
      } else {
        this.toast.error('Please check your form for errors');
      }
      return;
    }

    const { email, password } = this.signInForm.value;

    this.facade
      .login(email, password)
      .pipe(
        finalize(() => {
          // This will always run after success or error
          console.log('Request completed - loading should stop');
        })
      )
      .subscribe({
        next: () => {
          // The facade handles navigation and token storage.
          // If backend returned tokens immediately the facade navigates to '/'.
          // If OTP required the facade navigates to /auth/verify-otp.
          // So we don't need to handle navigation here.
        },
        error: (error) => {
          // Handle different types of errors with appropriate toast messages
          let errorMessage = 'An unexpected error occurred. Please try again.';

          // Handle HTTP error status codes
          if (error?.status === 401) {
            errorMessage = 'Invalid email or password. Please try again.';
          } else if (error?.status === 0) {
            errorMessage = 'Network error. Please check your connection.';
          } else if (error?.status >= 500) {
            errorMessage = 'Server error. Please try again later.';
          }
          // Handle custom API error response structure
          else if (error?.error && typeof error.error === 'object') {
            const apiError = error.error;
            if (apiError.message) errorMessage = apiError.message;
            if (apiError.statusCode === 'BadRequest' && apiError.message) {
              errorMessage = apiError.message;
            }
          } else if (error?.message) {
            errorMessage = error.message;
          }

          this.toast.error(errorMessage);
        },
      });
  }
}
