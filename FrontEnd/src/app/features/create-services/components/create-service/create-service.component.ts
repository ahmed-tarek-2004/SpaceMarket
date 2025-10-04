import { Component, EventEmitter, OnInit, Output, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CreateServiceFacadeService } from '../../services/create-service-facade.service';
import { ServiceCategory } from '../../../marketplace/interfaces/service-category';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-create-service',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-service.component.html',
  styleUrls: ['./create-service.component.scss'],
})
export class CreateServiceComponent implements OnInit {
  @Input() categories: ServiceCategory[] = [];
  @Output() imageSelected = new EventEmitter<File | null>();

  createServiceForm: FormGroup;
  selectedFile: File | null = null;
  imagePreview: string | ArrayBuffer | null = null;

  loadingCategories = false;
  categoriesError: string | null = null;

  // File validation errors
  fileError: string | null = null;

  // Facade observables
  isLoading$: Observable<boolean>;
  error$: Observable<string | null>;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private createServiceFacade: CreateServiceFacadeService,
    private toast: ToastService
  ) {
    this.createServiceForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      categoryId: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0)]],
      image: [null, Validators.required],
    });

    this.isLoading$ = this.createServiceFacade.loading$;
    this.error$ = this.createServiceFacade.error$;
  }

  ngOnInit(): void {}

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    this.fileError = null; // Clear previous errors

    if (!file) {
      this.selectedFile = null;
      this.imagePreview = null;
      this.createServiceForm.patchValue({ image: null });
      this.imageSelected.emit(null);
      return;
    }

    if (!this.validateFile(file)) return;

    this.selectedFile = file;
    this.createServiceForm.patchValue({ image: file });
    this.createServiceForm.get('image')?.setErrors(null); // Clear any previous errors

    // Preview and emit the file
    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreview = reader.result;
      this.imageSelected.emit(file);
    };
    reader.readAsDataURL(file);
  }

  public clearImageSelection() {
    this.selectedFile = null;
    this.imagePreview = null;
    this.fileError = null;
    this.createServiceForm.patchValue({ image: null });
    this.imageSelected.emit(null);
  }

  private validateFile(file: File): boolean {
    const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
    const maxSize = 5 * 1024 * 1024; // 5MB (reduced from 10MB)

    if (!validTypes.includes(file.type)) {
      this.fileError = 'Please select a valid image file (JPEG, PNG, GIF)';
      this.createServiceForm.get('image')?.setErrors({ invalidFileType: true });
      return false;
    }
    if (file.size > maxSize) {
      this.fileError = 'File size must be less than 5MB';
      this.createServiceForm.get('image')?.setErrors({ fileTooLarge: true });
      return false;
    }
    return true;
  }

  onSubmit(): void {
    // validation: form + file
    if (this.createServiceForm.invalid || !this.selectedFile) {
      this.markFormGroupTouched();

      if (!this.selectedFile) {
        this.fileError = 'Please select an image for your service';
        this.createServiceForm.get('image')?.setErrors({ required: true });
        // show a toast for validation failure (instead of alert)
        this.toast.error(this.fileError);
      } else {
        // generic validation error toast
        this.toast.error('Please fix form errors before submitting.');
      }

      return;
    }

    const formValue = this.createServiceForm.value;
    const payload = {
      Title: formValue.title,
      Description: formValue.description,
      CategoryId: formValue.categoryId,
      Price: formValue.price,
      Image: this.selectedFile,
    };

    this.createServiceFacade.createService(payload).subscribe({
      next: (response) => {
        // show a success toast and then navigate
        this.toast.success('Service created successfully. Pending admin approval.');
        // small delay allows toast to appear before route change (optional)
        // but keep it short so UX isn't blocked:
        setTimeout(() => {
          this.router.navigate(['/dashboard'], { queryParams: { created: 'true' } });
        }, 250);
      },
      error: (err) => {
        // try to read server-provided message, fallback to generic
        const serverMessage = err?.error?.message || err?.message;
        const toastMsg = serverMessage || 'Create service failed. Please try again.';
        this.toast.error(toastMsg);
        console.error('Create service error:', err);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/marketplace']);
  }

  private markFormGroupTouched(): void {
    Object.values(this.createServiceForm.controls).forEach((control) => control.markAsTouched());
  }
}
