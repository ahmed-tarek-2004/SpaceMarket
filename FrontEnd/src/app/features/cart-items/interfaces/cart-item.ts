export interface CartItem {
  cartItemId: string;
  serviceId: string;
  serviceTitle: string;
  providerName: string;
  imageUrl: string;
  quantity: number;
  unitPrice: number;
  total: number;
}
