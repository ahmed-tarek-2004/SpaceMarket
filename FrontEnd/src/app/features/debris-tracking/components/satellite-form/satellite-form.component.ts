import { Component, EventEmitter, Output, Input, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RegisterSatelliteRequest } from '../../interfaces/register-satellite-request';
import { Satellite } from '../../interfaces/satellite';
import { Router } from '@angular/router';

@Component({
  selector: 'app-satellite-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './satellite-form.component.html',
  styleUrls: ['./satellite-form.component.scss'],
})
export class SatelliteFormComponent implements OnInit {
  @Input() editMode = false;
  @Input() editData: { id: string; name: string; noradId: string } | null = null;
  @Output() formSubmit = new EventEmitter<RegisterSatelliteRequest>();
  private router = inject(Router);
  satelliteForm: FormGroup;
  isSubmitting = false;

  constructor(private fb: FormBuilder) {
    this.satelliteForm = this.fb.group({
      catalogSatelliteId: ['', [Validators.required, Validators.minLength(3)]],
      name: ['', [Validators.required, Validators.minLength(2)]],
      proximityThresholdKm: [5, [Validators.required, Validators.min(1), Validators.max(1000)]],
    });
  }

  ngOnInit(): void {
    if (this.editMode && this.editData) {
      this.satelliteForm.patchValue({
        catalogSatelliteId: this.editData.id,
        name: this.editData.name,
        proximityThresholdKm: 5, // Default threshold
      });

      // Disable the catalog satellite ID and name fields in edit mode
      this.satelliteForm.get('catalogSatelliteId')?.disable();
      this.satelliteForm.get('name')?.disable();
    } else if (this.editData) {
      // Pre-populate form with selected satellite data
      this.satelliteForm.patchValue({
        catalogSatelliteId: this.editData.id,
        name: this.editData.name,
        proximityThresholdKm: 5, // Default threshold
      });

      // Disable the catalog satellite ID and name fields since they're from the selected satellite
      this.satelliteForm.get('catalogSatelliteId')?.disable();
      this.satelliteForm.get('name')?.disable();
    }
  }

  onSubmit() {
    console.log('Form submission attempted');
    console.log('Form valid:', this.satelliteForm.valid);
    console.log('Edit mode:', this.editMode);
    console.log('Edit data:', this.editData);
    console.log(
      'Proximity threshold valid:',
      this.satelliteForm.get('proximityThresholdKm')?.valid
    );
    console.log(
      'Proximity threshold value:',
      this.satelliteForm.get('proximityThresholdKm')?.value
    );
    console.log('Is submitting:', this.isSubmitting);

    // In edit mode or when pre-populated, only validate the proximity threshold field
    const isFormValid =
      this.editMode || this.editData
        ? this.satelliteForm.get('proximityThresholdKm')?.valid
        : this.satelliteForm.valid;

    console.log('Is form valid:', isFormValid);

    if (isFormValid && !this.isSubmitting) {
      this.isSubmitting = true;
      console.log('Submitting form...');

      // Get form data, including disabled fields
      const formData: RegisterSatelliteRequest = {
        catalogSatelliteId:
          this.satelliteForm.get('catalogSatelliteId')?.value || this.editData?.id || '',
        name: this.satelliteForm.get('name')?.value || this.editData?.name || '',
        proximityThresholdKm: this.satelliteForm.get('proximityThresholdKm')?.value || 5,
      };

      console.log('Form data to emit:', formData);
      this.formSubmit.emit(formData);

      // Reset submitting state after a delay
      setTimeout(() => {
        this.isSubmitting = false;
      }, 1000);
    } else {
      console.log('Form validation failed, marking fields as touched');
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched() {
    Object.keys(this.satelliteForm.controls).forEach((key) => {
      const control = this.satelliteForm.get(key);
      control?.markAsTouched();
    });
  }

  resetForm() {
    this.satelliteForm.reset({
      catalogSatelliteId: '',
      name: '',
      proximityThresholdKm: 5,
    });
    this.isSubmitting = false;
  }

  getFieldError(fieldName: string): string {
    const control = this.satelliteForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['required']) {
        return `${this.getFieldLabel(fieldName)} is required`;
      }
      if (control.errors['minlength']) {
        return `${this.getFieldLabel(fieldName)} must be at least ${
          control.errors['minlength'].requiredLength
        } characters`;
      }
      if (control.errors['min']) {
        return `${this.getFieldLabel(fieldName)} must be at least ${control.errors['min'].min}`;
      }
      if (control.errors['max']) {
        return `${this.getFieldLabel(fieldName)} must be at most ${control.errors['max'].max}`;
      }
    }
    return '';
  }

  goBack() {
    this.router.navigate(['/client-dashboard']);
  }
  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      catalogSatelliteId: 'Catalog Satellite ID',
      name: 'Satellite Name',
      proximityThresholdKm: 'Proximity Threshold',
    };
    return labels[fieldName] || fieldName;
  }

  isFieldInvalid(fieldName: string): boolean {
    const control = this.satelliteForm.get(fieldName);
    return !!(control?.invalid && control?.touched);
  }
}
