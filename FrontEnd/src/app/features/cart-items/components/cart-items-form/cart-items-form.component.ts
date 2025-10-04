import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CartItemsService } from '../../services/cart-items.service';

@Component({
  selector: 'app-cart-items-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './cart-items-form.component.html',
  styleUrls: ['./cart-items-form.component.scss']
})
export class CartItemsFormComponent implements OnInit {
  cartForm!: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private cartService: CartItemsService
  ) {}

  ngOnInit(): void {
    this.cartForm = this.fb.group({
      serviceId: ['', Validators.required],
      dataSetId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
    });
  }

  onSubmit(): void {
    if (this.cartForm.valid) {
      this.isSubmitting = true;
      this.cartService.addToCart(this.cartForm.value).subscribe({
        next: (res) => {
          console.log('✅ Added to cart', res);
          this.cartForm.reset({ quantity: 1 }); 
        },
        error: (err) => {
          console.error('❌ Error adding to cart', err);
        },
        complete: () => {
          this.isSubmitting = false;
        }
      });
    }
  }

  getError(controlName: string): string {
    const labels: Record<string, string> = {
      serviceId: 'Service ID',
      dataSetId: 'DataSet ID',
      quantity: 'Quantity'
    };
    const label = labels[controlName] || controlName;
    const control = this.cartForm.get(controlName);
    if (control?.hasError('required')) return `${label} is required`;
    if (control?.hasError('min')) return `${label} must be at least 1`;
    return '';
  }
}