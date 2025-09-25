import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { SignInFacadeService } from '../../../services/sign-in/sign-in-facade.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-sign-in-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './sign-in-form.component.html',
  styleUrls: ['./sign-in-form.component.scss']
})
export class SignInFormComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  facade = inject(SignInFacadeService);

  signInForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  get email() {
    return this.signInForm.get('email');
  }

  get password() {
    return this.signInForm.get('password');
  }

  getError(control: any): string {
    if (control?.hasError('required')) return 'This field is required';
    if (control?.hasError('email')) return 'Please enter a valid email';
    if (control?.hasError('minlength')) return 'Password must be at least 6 characters';
    return '';
  }
  goToForgotPassword() {
  this.router.navigate(['/auth/forgot-password']);
}

onSubmit(): void {
  if (this.signInForm.invalid) return;

  const { email, password } = this.signInForm.value; 
  this.facade.login(email, password).subscribe(result => { 
    if (result) {
      this.router.navigate(['/']);
    }
  });

}
}

