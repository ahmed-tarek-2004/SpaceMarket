// src/app/features/service-detail/interfaces/service.interface.ts
export interface Review {
  author: string;
  rating: number;
  comment: string;
}

export interface Service {
  id: string;
  title: string;
  category: string;
  rating: number;
  reviewCount: number;
  coverage: string;
  deliveryTime: string;
  price: number;
  currency: string;
  previewImages: string[];
  description: string;
  useCases: string[];
  features: string[];
  whatsIncluded: string[];
  providerInfo: string;
  reviews: Review[]; 
}