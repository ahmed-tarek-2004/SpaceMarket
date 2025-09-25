import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { SignInFacadeService } from '../../../services/sign-in/sign-in-facade.service';


@Component({
  selector: 'app-reset-password-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password-form.component.html',
  styleUrls: ['./reset-password-form.component.scss']
})
export class ResetPasswordFormComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  facade = inject(SignInFacadeService);

  resetPasswordForm: FormGroup = this.fb.group({
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: this.passwordMatchValidator });

  isLoading = false;

  get newPassword() { return this.resetPasswordForm.get('newPassword'); }
  get confirmPassword() { return this.resetPasswordForm.get('confirmPassword'); }

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
  if (this.resetPasswordForm.invalid) return;
  const newPassword = this.resetPasswordForm.value.newPassword;
  const otp = localStorage.getItem('reset_otp')!; 
  this.facade.resetPassword(otp, newPassword).subscribe(result => {
    if (result) {
      localStorage.removeItem('reset_otp');
      this.router.navigate(['/auth/sign-in']);
    }
  });
}
}