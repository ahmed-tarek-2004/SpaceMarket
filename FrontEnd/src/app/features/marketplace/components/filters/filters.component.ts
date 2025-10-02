import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ServiceQuery } from '../../interfaces/service-query';
import { ServiceCategory } from '../../interfaces/service-category';
import { ServiceCategoryService } from '../../services/service-category-api.service';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-filters',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './filters.component.html',
  styleUrls: ['./filters.component.scss'],
})
export class FiltersComponent implements OnInit {
  @Output() filtersChanged = new EventEmitter<ServiceQuery>();

  // populated from backend
  categories: ServiceCategory[] = [];
  loadingCategories = false;
  categoriesError: string | null = null;

  // model (only fields the backend accepts)
  categoryId?: string | null = undefined;
  minPrice?: number | null = null;
  maxPrice?: number | null = null;
  location?: string = '';
  pageNumber = 1;
  pageSize = 12;

  constructor(private categoryService: ServiceCategoryService) {}

  ngOnInit(): void {
    this.loadCategories();
    // emit initial empty query to load first page if desired
    this.apply(); 
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

  apply() {
    const q: ServiceQuery = {
      CategoryId: this.categoryId || undefined,
      MinPrice: this.minPrice ?? undefined,
      MaxPrice: this.maxPrice ?? undefined,
      Location: this.location && this.location.trim().length ? this.location.trim() : undefined,
      PageNumber: this.pageNumber,
      PageSize: this.pageSize,
    };
    this.filtersChanged.emit(q);
  }

  reset() {
    this.categoryId = undefined;
    this.minPrice = null;
    this.maxPrice = null;
    this.location = '';
    this.pageNumber = 1;
    this.pageSize = 12;
    this.apply();
  }
}
