export interface DatasetDetailsResponse {
  id: string;
  title: string;
  description: string;
  categoryId: string;
  categoryName?: string;
  price: number;
  fileUrl?: string;
  thumbnailUrl?: string;
  providerId: string;
  providerName: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}
