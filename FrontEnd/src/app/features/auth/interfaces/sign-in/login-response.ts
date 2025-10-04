export interface LoginResponse {
  id: string;
  email: string;
  phoneNumber: string;
  role: string;
  isEmailConfirmed: boolean;
  accessToken: string;
  refreshToken: string;
  displayName: string;
}
