export interface DatasetListFilterRequest {
  categoryId?: string;
  providerId?: string;
  status?: string;
  minPrice?: number;
  maxPrice?: number;
  pageNumber?: number;
  pageSize?: number;
}
