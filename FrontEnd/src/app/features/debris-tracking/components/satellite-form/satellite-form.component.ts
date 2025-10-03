import { Component, EventEmitter, Output, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RegisterSatelliteRequest } from '../../interfaces/register-satellite-request';

@Component({
  selector: 'app-satellite-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './satellite-form.component.html',
  styleUrls: ['./satellite-form.component.scss'],
})
export class SatelliteFormComponent implements OnInit {
  @Input() editMode = false;
  @Input() editData: { id: string; name: string; proximityThresholdKm: number } | null = null;
  @Output() formSubmit = new EventEmitter<RegisterSatelliteRequest>();

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
        proximityThresholdKm: this.editData.proximityThresholdKm,
      });
    }
  }

  onSubmit() {
    if (this.satelliteForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;
      const formData: RegisterSatelliteRequest = this.satelliteForm.value;
      this.formSubmit.emit(formData);

      // Reset submitting state after a delay
      setTimeout(() => {
        this.isSubmitting = false;
      }, 1000);
    } else {
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
