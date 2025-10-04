import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ServiceCategory } from '../../interfaces/service-category';
import { ServiceCategoryService } from '../../services/service-category.service';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.scss']
})
export class CategoryListComponent implements OnInit {
  categories: ServiceCategory[] = [];
  deleteCategoryId: string | null = null;
  isLoading = false;
  errorMessage: string | null = null;

  @Output() editCategory = new EventEmitter<ServiceCategory>();

  constructor(private service: ServiceCategoryService) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.service.getCategories().subscribe({
      next: (res) => {
        debugger
        this.categories = res.data || [];
        if (this.categories.length === 0) {
          this.errorMessage = 'No categories found.';
        }
        this.isLoading = false;
      },
      error: (err) => {
        debugger
        console.error('Load categories error:', err);
        this.errorMessage = 'Failed to load categories.';
        this.isLoading = false;
      }
    });
  }

  onUpdate(id?: string): void {
    if (!id) return;
    this.service.getCategoryById(id).subscribe({
      next: (cat) => cat && this.editCategory.emit(cat),
      error: (err) => alert('Failed to load category: ' + (err.message || err))
    });
  }

  onDelete(id?: string): void {
    if (!id) return;
    this.deleteCategoryId = id;
  }

  confirmDelete(): void {
    if (!this.deleteCategoryId) return;

    this.service.deleteCategory(this.deleteCategoryId).subscribe({
      next: () => {
        this.loadCategories();
        this.cancelDelete();
      },
      error: (err) => {
        alert('Delete failed: ' + (err.message || err));
        this.cancelDelete();
      }
    });
  }

  cancelDelete(): void {
    this.deleteCategoryId = null;
  }

  isDeleting(id?: string): boolean {
    return this.deleteCategoryId === id;
  }
}
