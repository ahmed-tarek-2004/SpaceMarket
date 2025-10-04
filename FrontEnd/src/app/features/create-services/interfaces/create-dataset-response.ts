export interface CreateDatasetResponse {
  id: string;
  title: string;
  description: string;
  categoryId: string;
  price: number;
  fileUrl: string | null;
  thumbnailUrl: string | null;
  apiEndpoint: string | null;
  providerId: string;
  providerName: string;
  status: string;
  createdAt: string;
  UpdatedAt?: string;
}
