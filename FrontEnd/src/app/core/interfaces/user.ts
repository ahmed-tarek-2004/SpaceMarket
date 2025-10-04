export interface User {
  id: string;
  email: string;
  phoneNumber: string;
  role: string;
  isEmailConfirmed: boolean;
  accessToken: string;
  refreshToken: string;
}
