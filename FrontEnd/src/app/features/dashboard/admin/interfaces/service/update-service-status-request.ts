export interface UpdateServiceStatusRequest {
  serviceId: string;
  status: string;
  reason?: string;
}
