import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
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
import { BehaviorSubject, finalize } from 'rxjs';
import { ProviderSignUpFacadeService } from '../../../services/sign-up/provider/provider-sign-up-facade.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { Router } from '@angular/router';
import { ROUTES } from '../../../../../shared/config/constants';

@Component({
  selector: 'app-provider-sign-up-form',
  imports: [ReactiveFormsModule, InputComponent, ButtonComponent, AsyncPipe],
  templateUrl: './provider-sign-up-form.component.html',
  styleUrls: ['./provider-sign-up-form.component.scss'],
})
export class ProviderSignUpFormComponent implements OnInit {
  providerForm: FormGroup;
  private _isSubmitting = new BehaviorSubject<boolean>(false);
  isSubmitting$ = this._isSubmitting.asObservable();
  selectedFiles: File[] = [];

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  validationMessages = {
    email: {
      required: 'Email is required',
      email: 'Please enter a valid email address',
    },
    phoneNumber: {
      required: 'Phone number is required',
      pattern: 'Please enter a valid phone number (7-15 digits, optional + prefix)',
    },
    password: {
      required: 'Password is required',
      minlength: 'Password must be at least 8 characters long',
      pattern:
        'Password must include uppercase and lowercase letters, numbers, and special characters',
    },
    companyName: {
      required: 'Company name is required',
      pattern: 'Company name can only contain letters, numbers, spaces, and basic punctuation',
    },
    websiteUrl: {
      required: 'Website URL is required',
      pattern: 'Please enter a valid website URL (e.g., https://example.com)',
    },
    certificationFiles: {
      required: 'At least one certification image is required',
      maxSize: 'Image size should not exceed 10MB',
      invalidType: 'Only image files (PNG, JPG, JPEG, JFIF) are allowed',
      maxFiles: 'Maximum 5 files allowed',
    },
    confirmPassword: {
      required: 'Please confirm your password',
      passwordsMismatch: 'Passwords do not match',
    },
  };

  private readonly MAX_FILE_SIZE = 10 * 1024 * 1024; // 10MB
  private readonly MAX_FILES = 5;
  private readonly ALLOWED_TYPES = ['image/png', 'image/jpeg', 'image/jfif'];

  private isImage(file: File): boolean {
    const mimeOk = file.type && this.ALLOWED_TYPES.includes(file.type);
    if (mimeOk) return true;

    const name = (file.name || '').toLowerCase();
    return (
      name.endsWith('.png') ||
      name.endsWith('.jpg') ||
      name.endsWith('.jpeg') ||
      name.endsWith('.jfif')
    );
  }

  serverError: string | null = null;

  constructor(
    private fb: FormBuilder,
    private providerSignUpFacade: ProviderSignUpFacadeService,
    private toast: ToastService,
    private router: Router
  ) {
    this.providerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?\d{7,15}$/)]],
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
      companyName: [
        '',
        [Validators.required, Validators.pattern(/^[a-zA-Z0-9À-ÿ\u00f1\u00d1\s\-'.,&()]+$/)],
      ],
      websiteUrl: [
        '',
        [
          Validators.required,
          Validators.pattern(
            /https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)/
          ),
        ],
      ],
      certificationFiles: [null, [Validators.required, this.validateFiles.bind(this)]],
      confirmPassword: ['', [Validators.required, this.validatePasswordMatch.bind(this)]],
    });
  }

  ngOnInit() {
    // Clear top-level serverError when form changes
    this.providerForm.valueChanges.subscribe(() => {
      this.serverError = null;
    });

    // Re-validate confirm password when password changes
    this.providerForm.get('password')?.valueChanges.subscribe(() => {
      this.providerForm.get('confirmPassword')?.updateValueAndValidity();
    });

    // Remove server errors for a control when user edits that control
    Object.keys(this.providerForm.controls).forEach((k) => {
      const ctrl = this.providerForm.get(k);
      ctrl?.valueChanges.subscribe(() => {
        if (ctrl?.errors && (ctrl.errors as any)['server']) {
          const { server, ...rest } = ctrl.errors as any;
          ctrl.setErrors(Object.keys(rest).length ? rest : null);
        }
      });
    });
  }

  private validateFiles(_: AbstractControl): ValidationErrors | null {
    const files = this.selectedFiles ?? [];

    if (!files || files.length === 0) return { required: true };
    if (files.length > this.MAX_FILES) return { maxFiles: true };

    for (const file of files) {
      if (!this.isImage(file)) return { invalidType: true };
      if (file.size > this.MAX_FILE_SIZE) return { maxSize: true };
    }

    return null;
  }

  private validatePasswordMatch(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }

    const password = this.providerForm?.get('password')?.value;
    const confirmPassword = control.value;

    return password === confirmPassword ? null : { passwordsMismatch: true };
  }

  getValidationMessage(controlName: string): string {
    const control = this.providerForm.get(controlName);
    if (!control || !control.touched || !control.errors) return '';

    const errors = control.errors;

    // server-provided inline error (highest priority)
    if (errors['server']) {
      return errors['server'] as string;
    }

    if (controlName === 'certificationFiles') {
      if (errors['maxFiles']) {
        return this.validationMessages.certificationFiles.maxFiles;
      }
      if (errors['invalidType']) {
        return this.validationMessages.certificationFiles.invalidType;
      }
      if (errors['maxSize']) {
        return this.validationMessages.certificationFiles.maxSize;
      }
      if (errors['required']) {
        return this.validationMessages.certificationFiles.required;
      }
    }

    if (controlName === 'confirmPassword') {
      if (errors['passwordsMismatch']) {
        return this.validationMessages.confirmPassword.passwordsMismatch;
      }
      if (errors['required']) {
        return this.validationMessages.confirmPassword.required;
      }
    }

    const messages = this.validationMessages[controlName as keyof typeof this.validationMessages];
    if (messages) {
      for (const errorKey of Object.keys(errors)) {
        if (messages[errorKey as keyof typeof messages]) {
          return messages[errorKey as keyof typeof messages] as string;
        }
      }
    }

    return '';
  }

  shouldShowError(controlName: string): boolean {
    const control = this.providerForm.get(controlName);
    return !!(control && control.touched && control.invalid);
  }

  shouldShowPasswordStrength(): boolean {
    const passwordControl = this.providerForm.get('password');
    return !!(passwordControl?.value && passwordControl.touched);
  }

  onProviderSubmit() {
    this.providerForm.markAllAsTouched();
    this.serverError = null;

    if (this.providerForm.invalid) {
      this.scrollToFirstError();
      return;
    }

    const formValue = this.providerForm.value;

    const payload = {
      Email: formValue.email,
      PhoneNumber: formValue.phoneNumber,
      Password: formValue.password,
      CompanyName: formValue.companyName,
      WebsiteUrl: formValue.websiteUrl,
      CertificationFiles: this.selectedFiles,
    };

    this._isSubmitting.next(true);

    this.providerSignUpFacade
      .register(payload)
      .pipe(finalize(() => this._isSubmitting.next(false)))
      .subscribe({
        next: (res) => {
          this.toast.success('Provider account created successfully!');
          this.providerForm.reset();
          this.selectedFiles = [];
          if (this.fileInput?.nativeElement) this.fileInput.nativeElement.value = '';
          this.router.navigate([ROUTES.signIn]);
        },
        error: (err) => {
          // ask facade to interpret the error
          const errResult = this.providerSignUpFacade.mapError(err);
          const friendly = errResult?.message ?? 'An unexpected error occurred. Please try again.';

          // Clear previous server-side form errors (only 'server' keys)
          Object.keys(this.providerForm.controls).forEach((k) => {
            const c = this.providerForm.get(k);
            if (c && c.errors && (c.errors as any)['server']) {
              const { server, ...rest } = c.errors as any;
              c.setErrors(Object.keys(rest).length ? rest : null);
            }
          });

          if (errResult.field === 'phoneNumber') {
            const ctrl = this.providerForm.get('phoneNumber');
            ctrl?.setErrors({ ...(ctrl?.errors ?? {}), server: friendly });
            this.scrollToControl('phoneNumber');
            this.toast.error(friendly);
          } else if (errResult.field === 'email') {
            const ctrl = this.providerForm.get('email');
            ctrl?.setErrors({ ...(ctrl?.errors ?? {}), server: friendly });
            this.scrollToControl('email');
            this.toast.error(friendly);
          } else {
            // global error
            this.serverError = friendly;
            this.toast.error(friendly);
            this.scrollToFirstError();
          }
        },
      });
  }

  getServerErrorMessage(messageKey: string): string {
    const messages: Record<string, string> = {
      EMAIL_EXISTS: 'This email is already registered.',
      INVALID_PHONE: 'The phone number is invalid.',
      WEAK_PASSWORD: 'Your password does not meet security requirements.',
      SERVER_ERROR: 'A server error occurred. Please try again later.',
      'Phone number is already registered.': 'This phone number is already registered.',
    };

    return messages[messageKey] || 'An unexpected error occurred. Please try again.';
  }

  private scrollToFirstError(): void {
    const firstErrorElement = document.querySelector('.ng-invalid');
    if (firstErrorElement) {
      firstErrorElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
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

  onCertificationFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input?.files ? Array.from(input.files) : [];

    if (!files || files.length === 0) {
      if (this.fileInput?.nativeElement) this.fileInput.nativeElement.value = '';
      return;
    }

    const validFiles = this.validateNewFiles(files);

    if (validFiles.length > 0) {
      this.selectedFiles = [...this.selectedFiles, ...validFiles].slice(0, this.MAX_FILES);
    }

    this.providerForm.patchValue({
      certificationFiles: this.selectedFiles.length > 0 ? this.selectedFiles : null,
    });

    const ctrl = this.providerForm.get('certificationFiles');
    ctrl?.markAsTouched();
    ctrl?.updateValueAndValidity({ onlySelf: true, emitEvent: true });

    if (this.fileInput?.nativeElement) {
      this.fileInput.nativeElement.value = '';
    }
  }

  private validateNewFiles(files: File[]): File[] {
    const validFiles: File[] = [];

    for (const file of files) {
      if (this.selectedFiles.length + validFiles.length >= this.MAX_FILES) break;

      if (!this.isImage(file)) {
        console.warn(`Only image files allowed: ${file.name}`);
        continue;
      }

      if (file.size > this.MAX_FILE_SIZE) {
        console.warn(`File too large: ${file.name} (${this.formatFileSize(file.size)})`);
        continue;
      }

      const duplicateInSelected = this.selectedFiles.some(
        (f) => f.name === file.name && f.size === file.size
      );
      const duplicateInValid = validFiles.some((f) => f.name === file.name && f.size === file.size);

      if (!duplicateInSelected && !duplicateInValid) {
        validFiles.push(file);
      }
    }

    return validFiles;
  }

  removeFile(index: number): void {
    this.selectedFiles.splice(index, 1);

    this.providerForm.patchValue({
      certificationFiles: this.selectedFiles.length > 0 ? this.selectedFiles : null,
    });

    this.providerForm.get('certificationFiles')?.markAsTouched();
  }

  clearAllFiles(): void {
    this.selectedFiles = [];

    this.providerForm.patchValue({
      certificationFiles: null,
    });

    this.providerForm.get('certificationFiles')?.markAsTouched();

    if (this.fileInput?.nativeElement) {
      this.fileInput.nativeElement.value = '';
    }
  }

  getTotalFileSize(): string {
    const totalBytes = this.selectedFiles.reduce((total, file) => total + file.size, 0);
    return this.formatFileSize(totalBytes);
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  getPasswordStrengthPercentage(): number {
    const password = this.providerForm.get('password')?.value;
    if (!password) return 0;

    let strength = 0;

    if (password.length >= 8) strength += 25;
    if (/[a-z]/.test(password)) strength += 25;
    if (/[A-Z]/.test(password)) strength += 25;
    if (/[0-9]/.test(password)) strength += 15;
    if (/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) strength += 10;

    return Math.min(strength, 100);
  }

  getPasswordStrengthBarClass(): string {
    const percentage = this.getPasswordStrengthPercentage();

    if (percentage >= 80) return 'bg-green-500';
    if (percentage >= 60) return 'bg-blue-500';
    if (percentage >= 40) return 'bg-yellow-500';
    if (percentage >= 20) return 'bg-orange-500';
    return 'bg-red-500';
  }

  getPasswordStrength(): string {
    const percentage = this.getPasswordStrengthPercentage();

    if (percentage >= 80) return 'Very Strong';
    if (percentage >= 60) return 'Strong';
    if (percentage >= 40) return 'Good';
    if (percentage >= 20) return 'Weak';
    return 'Very Weak';
  }

  getPasswordStrengthClass(): string {
    const percentage = this.getPasswordStrengthPercentage();

    if (percentage >= 80) return 'text-green-600';
    if (percentage >= 60) return 'text-blue-600';
    if (percentage >= 40) return 'text-yellow-600';
    if (percentage >= 20) return 'text-orange-600';
    return 'text-red-600';
  }
}
