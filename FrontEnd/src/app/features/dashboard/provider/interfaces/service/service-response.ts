export interface ServiceResponse {
  id: string;
  title: string;
  description: string;
  price: number;
  imagesUrl: string;
  websiteUrl: string;
  categoryName: string;
  providerId: string;
  status: string;
  createdAt: string;
  updatedAt?: string;
  
  showDetails?: boolean;
  isEditing?: boolean;
  editData?: any;
}
