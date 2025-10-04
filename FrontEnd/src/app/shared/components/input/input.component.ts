import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EyePasswordComponent } from '../eye-password/eye-password.component';

@Component({
  selector: 'app-input',
  imports: [CommonModule, ReactiveFormsModule, EyePasswordComponent],
  templateUrl: './input.component.html',
  styleUrl: './input.component.scss'
})
export class InputComponent {
  @Input() parentFormGroup!: FormGroup;
  @Input() formGroup!: FormGroup;
  @Input() controlName!: string;
  @Input() label!: string;
  @Input() type: string = 'text';
  @Input() placeholder: string = '';
  @Input() icon!: string;
  @Input() required: boolean = false;
  @Input() disabled: boolean = false;
  @Input() errorMessage: string = '';

  showPassword = false;

  get control(): FormControl {
    return this.formGroup.get(this.controlName) as FormControl;
  }

  shouldShowError(): boolean {
    return !!this.errorMessage && this.control.invalid && this.control.touched;
  }
}