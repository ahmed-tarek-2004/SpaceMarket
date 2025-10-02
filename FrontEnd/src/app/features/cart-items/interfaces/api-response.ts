export interface ApiResponse<T> {
  statusCode: string;
  succeeded: boolean;
  message: string;
  errors: string[];
  data: T;
}
