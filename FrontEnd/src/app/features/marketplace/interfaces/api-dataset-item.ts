export interface ApiDatasetItem {
  id: string;
  title: string;
  description?: string;
  categoryId: string;
  categoryName?: string;
  providerName: string;
  price: number;
  thumbnailUrl?: string;
}
