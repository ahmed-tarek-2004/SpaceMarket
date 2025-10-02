import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ServiceCategory } from '../../../marketplace/interfaces/service-category';
import { ServiceCategoryService } from '../../../marketplace/services/service-category-api.service';
import { CreateServiceFacadeService } from '../../services/create-service-facade.service';
import { Observable, take } from 'rxjs';

@Component({
  selector: 'app-create-service-page',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './create-service-page.component.html',
  styleUrls: ['./create-service-page.component.scss'],
})
export class CreateServicePageComponent implements OnInit {
  createServiceForm: FormGroup;
  selectedFile: File | null = null;
  imagePreview: string | ArrayBuffer | null = null;

  categories: ServiceCategory[] = [];
  loadingCategories = false;
  categoriesError: string | null = null;

  // Observables from facade
  isLoading$: Observable<boolean>;
  error$: Observable<string | null>;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private categoryService: ServiceCategoryService,
    private createServiceFacade: CreateServiceFacadeService
  ) {
    this.createServiceForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      categoryId: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0)]],
      image: [null],
    });

    // Initialize observables in constructor
    this.isLoading$ = this.createServiceFacade.loading$;
    this.error$ = this.createServiceFacade.error$;
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  private loadCategories() {
    this.loadingCategories = true;
    this.categoriesError = null;

    this.categoryService
      .getCategories()
      .pipe(take(1))
      .subscribe({
        next: (cats) => {
          this.categories = cats;
          this.loadingCategories = false;
        },
        error: (err) => {
          console.error('Failed to load categories', err);
          this.categoriesError = 'Failed to load categories';
          this.loadingCategories = false;
        },
      });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // Validate file type and size
      if (!this.validateFile(file)) {
        return;
      }

      this.selectedFile = file;
      this.createServiceForm.patchValue({ image: file });

      // Create preview
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result;
      };
      reader.readAsDataURL(file);
    }
  }

  private validateFile(file: File): boolean {
    const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
    if (!validTypes.includes(file.type)) {
      alert('Please select a valid image file (JPEG, PNG, GIF)');
      return false;
    }

    const maxSize = 10 * 1024 * 1024; // 10MB
    if (file.size > maxSize) {
      alert('File size must be less than 10MB');
      return false;
    }

    return true;
  }

  get selectedCategoryName(): string {
    const categoryId = this.createServiceForm.get('categoryId')?.value;
    const category = this.categories.find((c) => c.id === categoryId);
    return category ? category.name : '';
  }

  getProviderInitial(): string {
    // This would typically come from user profile
    return 'Y';
  }

  onSubmit(): void {
    if (this.createServiceForm.valid && this.selectedFile) {
      const formValue = this.createServiceForm.value;

      const createServiceRequest = {
        Title: formValue.title,
        Description: formValue.description,
        CategoryId: formValue.categoryId,
        Price: formValue.price,
        Image: this.selectedFile,
      };

      // Only pass the request object, not the file separately
      this.createServiceFacade.createService(createServiceRequest).subscribe({
        next: (response) => {
          console.log('Service created successfully:', response);
          this.router.navigate(['/services'], {
            queryParams: { created: 'true' },
          });
        },
        error: (error) => {
          console.error('Create service error:', error);
        },
      });
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.createServiceForm.controls).forEach((key) => {
        this.createServiceForm.get(key)?.markAsTouched();
      });

      if (!this.selectedFile) {
        alert('Please select an image for your service');
      }
    }
  }

  onCancel(): void {
    this.router.navigate(['/marketplace']);
  }
}
