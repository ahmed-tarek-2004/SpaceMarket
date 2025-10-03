export interface ServiceResponse {
  id: string;
  title: string;
  description: string;
  providerId: string;
  providerName: string;
  providerEmail: string;
  categoryName: string;
  price: number;
  status: string;
  imagesUrl: string;
  createdAt: string;
  // Frontend only properties
  newStatus?: string;
  reason?: string;
  showReasonInput?: boolean;
  showDetails?: boolean;
}
