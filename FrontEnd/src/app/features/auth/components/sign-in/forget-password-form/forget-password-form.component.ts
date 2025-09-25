import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { SignInFacadeService } from '../../../services/sign-in/sign-in-facade.service';


@Component({
  selector: 'app-forget-password-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './forget-password-form.component.html',
  styleUrls: ['./forget-password-form.component.scss']
})
export class ForgetPasswordFormComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  facade = inject(SignInFacadeService);

  emailForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  isLoading = false;
  errorMessage: string | null = null;

  get email() {
    return this.emailForm.get('email');
  }
  goBack() {
  this.router.navigate(['/auth/sign-in']);
}

onSubmit(): void {
  if (this.emailForm.invalid) return;
  const email = this.emailForm.value.email;
  this.facade.forgetPassword(email).subscribe(result => {
    if (result) this.router.navigate(['/auth/verify-otp']);
  });
}
}