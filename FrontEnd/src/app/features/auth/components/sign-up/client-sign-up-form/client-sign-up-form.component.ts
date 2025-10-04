import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { InputComponent } from '../../../../../shared/components/input/input.component';
import { ButtonComponent } from '../../../../../shared/components/button/button.component';
import { ClientSignUpDetails } from '../../../interfaces/sign-up/client/client-sign-up-details';
import { ClientSignUpFacadeService } from '../../../services/sign-up/client/client-sign-up-facade.service';
import { BehaviorSubject, finalize } from 'rxjs';
import { ToastService } from '../../../../../shared/services/toast.service';
import { ROUTES } from '../../../../../shared/config/constants';
import { Router } from '@angular/router';

@Component({
  selector: 'app-client-sign-up-form',
  imports: [ReactiveFormsModule, InputComponent, ButtonComponent, AsyncPipe],
  templateUrl: './client-sign-up-form.component.html',
  styleUrl: './client-sign-up-form.component.scss',
})
export class ClientSignUpFormComponent implements OnInit {
  clientForm: FormGroup;
  private _isSubmitting = new BehaviorSubject<boolean>(false);
  isSubmitting$ = this._isSubmitting.asObservable();

  // Validation messages configuration
  validationMessages = {
    firstName: {
      required: 'First name is required',
      minlength: 'First name must be at least 2 characters long',
      pattern: 'First name can only contain letters, spaces, hyphens, and apostrophes',
    },
    lastName: {
      required: 'Last name is required',
      minlength: 'Last name must be at least 2 characters long',
      pattern: 'Last name can only contain letters, spaces, hyphens, and apostrophes',
    },
    email: {
      required: 'Email is required',
      email: 'Please enter a valid email address',
    },
    organization: {
      required: 'Organization name is required',
      pattern: 'Organization name can only contain letters, numbers, spaces, and basic punctuation',
    },
    phoneNumber: {
      required: 'Phone number is required',
      pattern: 'Please enter a valid phone number (7-15 digits, optional + prefix)',
    },
    country: {
      required: 'Country is required',
      pattern: 'Country name can only contain letters, spaces, hyphens, and apostrophes',
    },
    password: {
      required: 'Password is required',
      minlength: 'Password must be at least 8 characters long',
      pattern:
        'Password must include uppercase and lowercase letters, numbers, and special characters',
    },
    confirmPassword: {
      required: 'Please confirm your password',
      passwordsMismatch: 'Passwords do not match',
    },
  };

  // Server error messages
  serverError: string | null = null;

  constructor(
    private fb: FormBuilder,
    private signUpFacade: ClientSignUpFacadeService,
    private toast: ToastService,
    private router: Router
  ) {
    this.clientForm = this.fb.group({
      firstName: [
        '',
        [
          Validators.required,
          Validators.minLength(2),
          Validators.pattern(/^[a-zA-ZÀ-ÿ\u00f1\u00d1\s'-]+$/),
        ],
      ],
      lastName: [
        '',
        [
          Validators.required,
          Validators.minLength(2),
          Validators.pattern(/^[a-zA-ZÀ-ÿ\u00f1\u00d1\s'-]+$/),
        ],
      ],
      email: ['', [Validators.required, Validators.email]],
      organization: [
        '',
        [Validators.required, Validators.pattern(/^[a-zA-Z0-9À-ÿ\u00f1\u00d1\s\-'.,&()]+$/)],
      ],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?\d{7,15}$/)]],
      country: ['', [Validators.required, Validators.pattern(/^[a-zA-ZÀ-ÿ\u00f1\u00d1\s'-]+$/)]],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.pattern(
            /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])[A-Za-z\d!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]{8,}$/
          ),
        ],
      ],
      confirmPassword: ['', [Validators.required, this.validatePasswordMatch.bind(this)]],
    });
  }

  ngOnInit() {
    this.clientForm.valueChanges.subscribe(() => {
      this.serverError = null;
    });

    this.clientForm.get('password')?.valueChanges.subscribe(() => {
      this.clientForm.get('confirmPassword')?.updateValueAndValidity();
    });

    Object.keys(this.clientForm.controls).forEach((k) => {
      const ctrl = this.clientForm.get(k);
      ctrl?.valueChanges.subscribe(() => {
        if (ctrl?.errors && (ctrl.errors as any)['server']) {
          const { server, ...rest } = ctrl.errors as any;
          ctrl.setErrors(Object.keys(rest).length ? rest : null);
        }
      });
    });
  }

  // Form-level validator for password matching
  private validatePasswordMatch(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }

    const password = this.clientForm?.get('password')?.value;
    const confirmPassword = control.value;

    return password === confirmPassword ? null : { passwordsMismatch: true };
  }

  getValidationMessage(controlName: string): string {
    const control = this.clientForm.get(controlName);

    if (!control || !control.touched) return '';

    // Handle confirmPassword errors
    if (controlName === 'confirmPassword') {
      if (control.errors?.['passwordsMismatch']) {
        return this.validationMessages.confirmPassword.passwordsMismatch;
      }
      if (control.errors?.['required']) {
        return this.validationMessages.confirmPassword.required;
      }
    }

    // Handle other controls...
    if (control.errors) {
      const messages = this.validationMessages[controlName as keyof typeof this.validationMessages];
      if (messages) {
        for (const errorKey of Object.keys(control.errors)) {
          if (messages[errorKey as keyof typeof messages]) {
            return messages[errorKey as keyof typeof messages] as string;
          }
        }
      }
    }

    return '';
  }

  // Check if field should show error
  shouldShowError(controlName: string): boolean {
    const control = this.clientForm.get(controlName);
    return !!(control && control.touched && control.invalid);
  }

  // Check if password field is touched and has value
  shouldShowPasswordStrength(): boolean {
    const passwordControl = this.clientForm.get('password');
    return !!(passwordControl?.value && passwordControl.touched);
  }

  onClientSubmit() {
    this.clientForm.markAllAsTouched();
    this.serverError = null;

    if (this.clientForm.invalid) {
      this.scrollToFirstError();
      return;
    }

    const formValue = this.clientForm.value;

    const payload: ClientSignUpDetails = {
      FullName: `${formValue.firstName} ${formValue.lastName}`,
      OrganizationName: formValue.organization,
      Email: formValue.email,
      PhoneNumber: formValue.phoneNumber,
      Password: formValue.password,
      ConfirmPassword: formValue.confirmPassword,
      Country: formValue.country,
    };

    this._isSubmitting.next(true);

    this.signUpFacade
      .register(payload)
      .pipe(finalize(() => this._isSubmitting.next(false)))
      .subscribe({
        next: (res) => {
          this.toast.success('Account created successfully!');
          console.log('Success:', res);
          this.clientForm.reset();
          this.router.navigate([ROUTES.signIn]);
        },
        error: (err) => {
          // Get structured result — message is raw backend message (no localization)
          const errResult = this.signUpFacade.mapError(err);
          const friendly = errResult?.message ?? 'An unexpected error occurred. Please try again.';

          // Clear previous 'server' errors only
          Object.keys(this.clientForm.controls).forEach((k) => {
            const c = this.clientForm.get(k);
            if (c && c.errors && (c.errors as any)['server']) {
              const { server, ...rest } = c.errors as any;
              c.setErrors(Object.keys(rest).length ? rest : null);
            }
          });

          if (errResult.field === 'phoneNumber') {
            const ctrl = this.clientForm.get('phoneNumber');
            ctrl?.setErrors({ ...(ctrl?.errors ?? {}), server: friendly });
            this.scrollToControl('phoneNumber');
            this.toast.error(friendly);
          } else if (errResult.field === 'email') {
            const ctrl = this.clientForm.get('email');
            ctrl?.setErrors({ ...(ctrl?.errors ?? {}), server: friendly });
            this.scrollToControl('email');
            this.toast.error(friendly);
          } else {
            // global error: keep top-level serverError (for banner) and show toast
            this.serverError = friendly;
            this.toast.error(friendly);
            this.scrollToFirstError();
          }
        },
      });
  }

  private scrollToControl(controlName: string) {
    const el =
      document.querySelector(`[formcontrolname="${controlName}"]`) ??
      document.querySelector(`[controlname="${controlName}"]`) ??
      document.getElementById(controlName) ??
      document.querySelector('.ng-invalid');

    if (el) {
      (el as HTMLElement).scrollIntoView({ behavior: 'smooth', block: 'center' });
      const input = (el as HTMLElement).querySelector(
        'input, textarea, [contenteditable="true"]'
      ) as HTMLElement | null;
      if (input) input.focus();
    }
  }

  private getServerErrorMessage(messageKey: string): string {
    const errorMessages: { [key: string]: string } = {
      EMAIL_ALREADY_EXISTS: 'This email is already registered',
      INVALID_EMAIL: 'Please enter a valid email address',
      WEAK_PASSWORD: 'Password is too weak',
      NETWORK_ERROR: 'Network error. Please check your connection',
      UNKNOWN_ERROR: 'An unexpected error occurred. Please try again.',
    };

    return errorMessages[messageKey] || 'An error occurred during registration';
  }

  private scrollToFirstError(): void {
    const firstErrorElement = document.querySelector('.ng-invalid');
    if (firstErrorElement) {
      firstErrorElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
  }

  get f() {
    return this.clientForm.controls;
  }

  // Enhanced password strength with percentage
  getPasswordStrengthPercentage(): number {
    const password = this.clientForm.get('password')?.value;
    if (!password) return 0;

    let strength = 0;

    if (password.length >= 8) strength += 25;
    if (/[a-z]/.test(password)) strength += 25;
    if (/[A-Z]/.test(password)) strength += 25;
    if (/[0-9]/.test(password)) strength += 15;
    if (/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) strength += 10;

    return Math.min(strength, 100);
  }

  // Enhanced password strength bar colors
  getPasswordStrengthBarClass(): string {
    const percentage = this.getPasswordStrengthPercentage();

    if (percentage >= 80) return 'bg-green-500';
    if (percentage >= 60) return 'bg-blue-500';
    if (percentage >= 40) return 'bg-yellow-500';
    if (percentage >= 20) return 'bg-orange-500';
    return 'bg-red-500';
  }

  // Enhanced password strength text
  getPasswordStrength(): string {
    const percentage = this.getPasswordStrengthPercentage();

    if (percentage >= 80) return 'Very Strong';
    if (percentage >= 60) return 'Strong';
    if (percentage >= 40) return 'Good';
    if (percentage >= 20) return 'Weak';
    return 'Very Weak';
  }

  // Enhanced password strength color classes
  getPasswordStrengthClass(): string {
    const percentage = this.getPasswordStrengthPercentage();

    if (percentage >= 80) return 'text-green-600';
    if (percentage >= 60) return 'text-blue-600';
    if (percentage >= 40) return 'text-yellow-600';
    if (percentage >= 20) return 'text-orange-600';
    return 'text-red-600';
  }
}
