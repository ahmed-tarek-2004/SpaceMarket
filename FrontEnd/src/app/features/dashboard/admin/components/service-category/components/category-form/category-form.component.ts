// src/app/components/category-form/category-form.component.ts
import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ServiceCategory } from '../../interfaces/service-category';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './category-form.component.html',
  styleUrls: ['./category-form.component.scss']
})
export class CategoryFormComponent implements OnInit {
  @Input() mode: 'create' | 'update' = 'create';
  @Input() category: ServiceCategory | null = null;
  @Output() categorySaved = new EventEmitter<ServiceCategory>();
  @Output() cancel = new EventEmitter<void>();

  form!: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    debugger
  this.form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    description: ['', [Validators.required, Validators.minLength(5)]]
  });

  if (this.mode === 'update' && this.category) {
    this.form.patchValue({
      name: this.category.name,
      description: this.category.description
    });
  }
}

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.categorySaved.emit(this.form.value);
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
