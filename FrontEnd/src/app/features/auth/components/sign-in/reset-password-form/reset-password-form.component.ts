import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { SignInFacadeService } from '../../../services/sign-in/sign-in-facade.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { finalize } from 'rxjs';
import { ButtonComponent } from '../../../../../shared/components/button/button.component';
import { InputComponent } from '../../../../../shared/components/input/input.component';

@Component({
  selector: 'app-reset-password-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, InputComponent],
  templateUrl: './reset-password-form.component.html',
  styleUrls: ['./reset-password-form.component.scss'],
})
export class ResetPasswordFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toast = inject(ToastService);
  facade = inject(SignInFacadeService);

  resetPasswordForm: FormGroup = this.fb.group(
    {
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: this.passwordMatchValidator }
  );

  isLoading = false;
  userId: string | null = null;
  otp: string | null = null;

  get newPassword() {
    return this.resetPasswordForm.get('newPassword');
  }
  get confirmPassword() {
    return this.resetPasswordForm.get('confirmPassword');
  }

  ngOnInit() {
    // Get userId and OTP from query params
    this.route.queryParams.subscribe((params) => {
      this.userId = params['userId'] || this.facade.getCurrentUserId() || null;
      this.otp = params['otp'] || null;

      if (!this.userId || !this.otp) {
        this.toast.error('Invalid reset password link. Please try again.');
        this.router.navigate(['/auth/forgot-password']);
      }
    });
  }

  passwordMatchValidator(form: FormGroup) {
    const newPass = form.get('newPassword')?.value;
    const confirmPass = form.get('confirmPassword')?.value;
    return newPass === confirmPass ? null : { mismatch: true };
  }

  getNewPasswordError(): string {
    const ctrl = this.newPassword;
    if (ctrl?.hasError('required')) return 'New password is required';
    if (ctrl?.hasError('minlength')) return 'Password must be at least 6 characters';
    return '';
  }

  getConfirmPasswordError(): string {
    const ctrl = this.confirmPassword;
    if (ctrl?.hasError('required')) return 'Please confirm your new password';
    if (this.resetPasswordForm.hasError('mismatch')) return 'Passwords do not match';
    return '';
  }

  onSubmit(): void {
    this.resetPasswordForm.markAllAsTouched();

    if (this.resetPasswordForm.invalid) {
      if (this.resetPasswordForm.hasError('mismatch')) {
        this.toast.error('Passwords do not match');
      } else {
        this.toast.error('Please fill in all required fields correctly');
      }
      return;
    }

    if (!this.otp) {
      this.toast.error('OTP not found. Please try the reset process again.');
      return;
    }

    const newPassword = this.resetPasswordForm.value.newPassword;
    this.isLoading = true;

    this.facade
      .resetPassword(this.otp, newPassword)
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe({
        next: () => {
          this.toast.success('Password reset successfully!');
          this.router.navigate(['/auth/sign-in'], {
            queryParams: { reset: true },
          });
        },
        error: (error) => {
          let errorMessage = 'Failed to reset password';

          if (error?.error && typeof error.error === 'object') {
            const apiError = error.error;
            if (apiError.message) errorMessage = apiError.message;
          } else if (error?.message) {
            errorMessage = error.message;
          }

          this.toast.error(errorMessage);
        },
      });
  }

  goBack(): void {
    this.router.navigate(['/auth/forgot-password']);
  }
}
