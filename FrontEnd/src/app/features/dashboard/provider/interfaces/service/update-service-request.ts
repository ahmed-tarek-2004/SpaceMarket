export interface UpdateServiceRequest {
  id: string;
  title?: string;
  description?: string;
  categoryId?: string;
  price?: number;
  image?: File;
}
