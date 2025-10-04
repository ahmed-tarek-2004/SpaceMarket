export interface UpdateDatasetStatusRequest {
  datasetId: string;
  status: string;
  reason?: string;
}
