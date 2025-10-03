export interface DatasetResponse {
  id: string;
  title: string;
  description: string;
  providerId: string;
  providerName: string;
  providerEmail: string;
  categoryName: string;
  price: number;
  status: string;
  thumbnailUrl: string;
  createdAt: string;

  // UI-only properties
  newStatus?: string;
  reason?: string;
  showReasonInput?: boolean;
  showDetails?: boolean;
}
