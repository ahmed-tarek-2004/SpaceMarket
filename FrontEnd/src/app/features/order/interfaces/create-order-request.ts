export interface CreateOrderRequest {
  clientId: string | null;
  orderItem: {
    itemId: string | undefined;
    type: string;
    priceSnapshot: number;
  };
}
