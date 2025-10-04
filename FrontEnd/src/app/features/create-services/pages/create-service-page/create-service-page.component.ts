import {
  Component,
  OnInit,
  ViewChild,
  AfterViewInit,
  OnDestroy,
  ChangeDetectorRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { CreateServiceComponent } from '../../components/create-service/create-service.component';
import { CreateDatasetComponent } from '../../components/create-dataset/create-dataset.component';
import { Observable, Subscription, take, BehaviorSubject } from 'rxjs';
import { ServiceCategory } from '../../../marketplace/interfaces/service-category';
import { ServiceCategoryService } from '../../../marketplace/services/service-category-api.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser'; // Add this import

@Component({
  selector: 'app-create-service-page',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, CreateServiceComponent, CreateDatasetComponent],
  templateUrl: './create-service-page.component.html',
  styleUrls: ['./create-service-page.component.scss'],
})
export class CreateServicePageComponent implements OnInit, AfterViewInit, OnDestroy {
  tab: 'createService' | 'createDataset' | 'services' = 'createService';

  @ViewChild(CreateServiceComponent) createServiceComponent!: CreateServiceComponent;
  @ViewChild(CreateDatasetComponent) createDatasetComponent!: CreateDatasetComponent;

  imagePreview: string | ArrayBuffer | null = null;
  datasetThumbnailPreview: SafeUrl | null = null; // Add this

  categories: ServiceCategory[] = [];
  loadingCategories = false;
  categoriesError: string | null = null;

  // Add BehaviorSubjects to track form values
  serviceFormValues = new BehaviorSubject<any>({});
  datasetFormValues = new BehaviorSubject<any>({});

  private subs = new Subscription();

  constructor(
    private categoryService: ServiceCategoryService,
    private cdRef: ChangeDetectorRef,
    private sanitizer: DomSanitizer // Add this
  ) {}

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

  ngAfterViewInit(): void {
    console.log('ngAfterViewInit - Setting up imageSelected subscription');

    // Safe subscription to imageSelected event
    if (this.createServiceComponent?.imageSelected) {
      const s = this.createServiceComponent.imageSelected.subscribe((file) => {
        console.log('Received file in parent:', file);
        if (file instanceof File) {
          this.loadPreviewFromFile(file);
        } else {
          this.imagePreview = null;
          this.cdRef.detectChanges();
        }
      });
      this.subs.add(s);
    } else {
      console.log('createServiceComponent or imageSelected not available');
    }

    // Subscribe to dataset thumbnail preview changes
    if (this.createDatasetComponent?.thumbnailPreviewChanged) {
      const thumbnailSub = this.createDatasetComponent.thumbnailPreviewChanged.subscribe(
        (preview) => {
          this.datasetThumbnailPreview = preview;
          this.cdRef.detectChanges();
        }
      );
      this.subs.add(thumbnailSub);
    }

    // Subscribe to dataset form value changes
    if (this.createDatasetComponent?.formValuesChanged) {
      const formValuesSub = this.createDatasetComponent.formValuesChanged.subscribe((values) => {
        this.datasetFormValues.next(values);
        this.cdRef.detectChanges();
      });
      this.subs.add(formValuesSub);
    }

    // Subscribe to form value changes for real-time updates
    this.setupFormListeners();
  }

  private setupFormListeners() {
    // Listen to service form changes
    if (this.createServiceComponent?.createServiceForm) {
      const serviceFormSub = this.createServiceComponent.createServiceForm.valueChanges.subscribe(
        (values) => {
          this.serviceFormValues.next(values);
        }
      );
      this.subs.add(serviceFormSub);
    }

    // Listen to dataset form changes
    if (this.createDatasetComponent?.createDatasetForm) {
      const datasetFormSub = this.createDatasetComponent.createDatasetForm.valueChanges.subscribe(
        (values) => {
          this.datasetFormValues.next(values);
        }
      );
      this.subs.add(datasetFormSub);
    }
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  setTab(tabName: 'createService' | 'createDataset' | 'services') {
    this.tab = tabName;
    this.imagePreview = null;
    this.datasetThumbnailPreview = null; // Reset dataset thumbnail when switching tabs

    // Clear image selection when switching away from createService
    if (tabName !== 'createService' && this.createServiceComponent?.clearImageSelection) {
      this.createServiceComponent.clearImageSelection();
    }

    // Re-setup form listeners when switching tabs (in case components weren't ready)
    setTimeout(() => this.setupFormListeners(), 0);
  }

  // Update getters to use BehaviorSubjects
  get currentServiceFormValues() {
    return this.serviceFormValues.value;
  }

  get currentDatasetFormValues() {
    return this.datasetFormValues.value;
  }

  getProviderInitial(): string {
    return 'Y'; // Or get from user profile
  }

  get selectedCategoryName(): string {
    let categoryId: string | null = null;

    if (this.tab === 'createService') {
      categoryId = this.currentServiceFormValues.categoryId;
    } else if (this.tab === 'createDataset') {
      categoryId = this.currentDatasetFormValues.categoryId;
    }

    const category = this.categories.find((cat) => cat.id === categoryId);
    return category?.name || '';
  }

  get datasetFileName(): string {
    return this.createDatasetComponent?.datasetFileName || '';
  }

  getFileExtension(filename: string): string {
    return filename.split('.').pop()?.toUpperCase() || 'FILE';
  }

  private loadPreviewFromFile(file: File) {
    console.log('Loading preview from file:', file.name);
    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreview = reader.result;
      console.log('Preview loaded, imagePreview set to:', this.imagePreview ? 'data URL' : 'null');
      this.cdRef.detectChanges();
    };
    reader.onerror = (error) => {
      console.error('Error reading file:', error);
    };
    reader.readAsDataURL(file);
  }

  get error$(): Observable<string | null> | null {
    if (this.tab === 'createService') {
      return this.createServiceComponent?.error$ ?? null;
    }
    if (this.tab === 'createDataset') {
      return this.createDatasetComponent?.error$ ?? null;
    }
    return null;
  }

  // Add method to get dataset thumbnail preview for template
  getDatasetThumbnailPreview(): SafeUrl | string {
    return this.datasetThumbnailPreview || '';
  }
  
  onThumbnailPreviewChanged(preview: SafeUrl | null) {
    console.log('parent received thumbnail preview', preview);
    this.datasetThumbnailPreview = preview;
    this.cdRef.detectChanges();
  }
}
