export interface CartItemResponse {
  cartItemId: string;
  itemType: string;
  itemId?: string;
  title: string;
  unitPrice: number;
  total: number;
  commissionPercent?: number;
  commissionAmount?: number;
  providerAmount: number;
  providerName?: string;
  imageUrl?: string;
}
