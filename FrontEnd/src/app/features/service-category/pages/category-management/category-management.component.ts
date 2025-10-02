import { Component, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CategoryFormComponent } from '../../components/category-form/category-form.component';
import { CategoryListComponent } from '../../components/category-list/category-list.component';
import { ServiceCategory } from '../../interfaces/service-category';
import { ServiceCategoryService } from '../../services/service-category.service';
import { HttpErrorResponse } from '@angular/common/http';


@Component({
  selector: 'app-category-management-page',
  standalone: true,
  imports: [CommonModule, CategoryFormComponent, CategoryListComponent],
  templateUrl: './category-management.component.html',
  styleUrls: ['./category-management.component.scss']
})
export class CategoryManagementPageComponent {
  showForm = signal<boolean>(false);
  formMode: 'create' | 'update' = 'create';
  selectedCategory: ServiceCategory | null = null;

@ViewChild(CategoryListComponent, { static: false }) listCmp!: CategoryListComponent;
  

  constructor(private service: ServiceCategoryService) {}

  onAddNew(): void {
    debugger
    this.formMode = 'create';
    this.selectedCategory = null;
    this.showForm.set(true);
  }

  onEditCategory(category: ServiceCategory): void {
    this.formMode = 'update';
    this.selectedCategory = category;
    this.showForm.set(true);
  }

  onFormSaved(categoryData: ServiceCategory): void {
  if (this.formMode === 'create') {
    this.service.createCategory(categoryData).subscribe({
      next: () => {
        this.showForm.set(false);
        setTimeout(() => this.listCmp?.loadCategories(), 0);
      },
      error: (err: HttpErrorResponse) => {
        alert('Create failed: ' + (err.error?.message || err.message));
      }
    });
  }
  else if (this.formMode === 'update' && this.selectedCategory?.id) {
    this.service.updateCategory(this.selectedCategory.id, categoryData).subscribe({
      next: () => {
        this.showForm.set(false);
        setTimeout(() => this.listCmp?.loadCategories(), 0);
      },
      error: (err: HttpErrorResponse) => {
        alert('Update failed: ' + (err.error?.message || err.message));
      }
    });
  }
}


  onFormCancel(): void {
   this.showForm.set(false);
  }
} 