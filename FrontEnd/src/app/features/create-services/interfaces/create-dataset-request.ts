export interface CreateDatasetRequest {
  title: string;
  description?: string;
  categoryId: string;
  price: number;
  file?: string;
  thumbnail?: string;
  apiEndpoint?: string;
}
