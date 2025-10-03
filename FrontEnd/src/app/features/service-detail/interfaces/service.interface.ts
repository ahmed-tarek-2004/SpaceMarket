export interface ServiceDetailsResponse {
  id: string;
  title: string;
  description: string;
  price: number;
  imagesUrl: string;
  categoryName: string;
  providerId: string;
  status: string;
  createdAt: string;
  updatedAt?: string;
}
