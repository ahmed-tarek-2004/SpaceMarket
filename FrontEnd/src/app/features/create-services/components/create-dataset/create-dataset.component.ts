import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CreateDatasetFacadeService } from '../../services/create-dataset-facade.service';
import { CreateDatasetRequest } from '../../interfaces/create-dataset-request';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-create-dataset',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-dataset.component.html',
  styleUrls: ['./create-dataset.component.scss'],
})
export class CreateDatasetComponent implements OnInit {
  @Input() categories: any[] = [];
  @Output() datasetCreated = new EventEmitter<any>();
  @Output() cancelled = new EventEmitter<void>();
  @Output() thumbnailPreviewChanged = new EventEmitter<SafeUrl | null>();
  @Output() formValuesChanged = new EventEmitter<any>();

  createDatasetForm: FormGroup;
  selectedFile: File | null = null;
  selectedThumbnail: File | null = null;
  datasetFileName: string = '';
  thumbnailFileName: string = '';
  thumbnailPreview: SafeUrl | null = null;

  // Current form values for preview
  currentDatasetFormValues: any = {
    title: '',
    description: '',
    price: 0,
    categoryId: '',
  };

  selectedCategoryName: string = '';

  // Facade observables
  isLoading$: Observable<boolean>;
  error$: Observable<string | null>;

  constructor(
    private fb: FormBuilder,
    private facade: CreateDatasetFacadeService,
    private sanitizer: DomSanitizer,
    private toastService: ToastService, 
    private router: Router 
  ) {
    this.isLoading$ = this.facade.loading$;
    this.error$ = this.facade.error$;

    this.createDatasetForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      categoryId: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0)]],
      apiEndpoint: [''], // Optional field
    });
  }

  ngOnInit() {
    this.facade.clearError();

    // Listen to form changes for real-time preview
    this.createDatasetForm.valueChanges.subscribe((values) => {
      this.currentDatasetFormValues = { ...values };
      this.updateSelectedCategoryName();

      // Emit form values to parent
      this.formValuesChanged.emit({
        ...this.currentDatasetFormValues,
        categoryName: this.selectedCategoryName,
      });
    });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // Validate file size (50MB limit)
      if (file.size > 50 * 1024 * 1024) {
        // Fixed: Changed from 5MB to 50MB
        this.showFileError('File size must be less than 50MB');
        event.target.value = ''; // Clear the file input
        return;
      }

      // Validate file type
      const allowedTypes = ['.jpg', '.jpeg', '.png', '.gif', '.webp', '.bmp', '.tiff', '.svg'];
      const fileExtension = '.' + file.name.split('.').pop()?.toLowerCase();

      if (!allowedTypes.includes(fileExtension)) {
        this.showFileError(
          'Please upload a valid image file (JPG, PNG, GIF, WEBP, BMP, TIFF, SVG)'
        );
        event.target.value = ''; // Clear the file input
        return;
      }

      this.selectedFile = file;
      this.datasetFileName = file.name;
    }
  }

  onThumbnailSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // Validate file size (5MB limit for thumbnails)
      if (file.size > 5 * 1024 * 1024) {
        this.showFileError('Thumbnail size must be less than 5MB');
        event.target.value = ''; // Clear the file input
        return;
      }

      // Validate image type
      const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp', '.bmp', '.tiff', '.svg'];
      const fileExtension = '.' + file.name.split('.').pop()?.toLowerCase();

      if (!allowedExtensions.includes(fileExtension)) {
        this.showFileError(
          'Please upload a valid image file (JPG, PNG, GIF, WEBP, BMP, TIFF, SVG)'
        );
        event.target.value = ''; // Clear the file input
        return;
      }

      this.selectedThumbnail = file;
      this.thumbnailFileName = file.name;

      // Create preview and emit to parent
      this.createThumbnailPreview(file);
    }
  }

  createThumbnailPreview(file: File): void {
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.thumbnailPreview = this.sanitizer.bypassSecurityTrustUrl(e.target.result);
      // Emit thumbnail preview to parent
      this.thumbnailPreviewChanged.emit(this.thumbnailPreview);
    };
    reader.readAsDataURL(file);
  }

  getThumbnailPreview(): SafeUrl | string {
    return this.thumbnailPreview || '';
  }

  getProviderInitial(): string {
    // You can replace this with actual provider name logic
    return 'Y';
  }

  updateSelectedCategoryName(): void {
    const categoryId = this.createDatasetForm.get('categoryId')?.value;
    if (categoryId && this.categories) {
      const category = this.categories.find((cat) => cat.id === categoryId);
      this.selectedCategoryName = category ? category.name : '';
    } else {
      this.selectedCategoryName = '';
    }
  }

  onSubmitDataset(): void {
    if (this.createDatasetForm.valid && this.selectedFile) {
      const datasetRequest: CreateDatasetRequest = {
        title: this.createDatasetForm.get('title')?.value,
        description: this.createDatasetForm.get('description')?.value,
        categoryId: this.createDatasetForm.get('categoryId')?.value,
        price: parseFloat(this.createDatasetForm.get('price')?.value) || 0,
        apiEndpoint: this.createDatasetForm.get('apiEndpoint')?.value || undefined,
      };

      // Convert null to undefined for the thumbnail parameter
      const thumbnailFile = this.selectedThumbnail || undefined;

      this.facade.createDataset(datasetRequest, this.selectedFile, thumbnailFile).subscribe({
        next: (response) => {
          // Show success toast
          this.toastService.success('Dataset created successfully!');

          // Emit the created dataset
          this.datasetCreated.emit(response);

          // Reset the form
          this.resetDatasetForm();

          // Navigate to dashboard after a short delay to show the toast
          setTimeout(() => {
            this.router.navigate(['/dashboard']);
          }, 1000);
        },
        error: (error) => {
          console.error('Error creating dataset:', error);
          // Show error toast - user stays on the current page
          this.toastService.error('Failed to create dataset. Please try again.');
        },
      });
    } else {
      this.markFormGroupTouched();
      if (!this.selectedFile) {
        this.showFileError('Please select a dataset file');
      }
    }
  }

  resetDatasetForm(): void {
    this.createDatasetForm.reset();
    this.selectedFile = null;
    this.selectedThumbnail = null;
    this.datasetFileName = '';
    this.thumbnailFileName = '';
    this.thumbnailPreview = null;
    this.currentDatasetFormValues = {
      title: '',
      description: '',
      price: 0,
      categoryId: '',
    };
    this.selectedCategoryName = '';

    // Emit reset to parent
    this.thumbnailPreviewChanged.emit(null);
    this.formValuesChanged.emit({
      title: '',
      description: '',
      price: 0,
      categoryId: '',
      categoryName: '',
    });

    this.facade.clearError();
  }

  cancelCreation(): void {
    this.resetDatasetForm();
    this.cancelled.emit();
  }

  private markFormGroupTouched(): void {
    Object.keys(this.createDatasetForm.controls).forEach((key) => {
      const control = this.createDatasetForm.get(key);
      control?.markAsTouched();
    });
  }

  hasError(controlName: string, errorType: string): boolean {
    const control = this.createDatasetForm.get(controlName);
    return (control?.hasError(errorType) ?? false) && (control?.touched ?? false);
  }

  private showFileError(message: string): void {
    // Use toast service instead of alert
    this.toastService.error(message);
  }
}
