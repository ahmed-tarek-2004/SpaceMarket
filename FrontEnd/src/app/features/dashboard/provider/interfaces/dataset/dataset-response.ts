export interface DatasetResponse {
  id: string;
  title: string;
  description: string;
  categoryId: string;
  categoryName: string;
  price: number;
  fileUrl: string;
  thumbnailUrl: string;
  apiEndpoint: string;
  providerId: string;
  providerName: string;
  status: string;
  createdAt: string;
  updatedAt?: string;

  showDetails?: boolean;
  isEditing?: boolean;
  editData?: any;
}
