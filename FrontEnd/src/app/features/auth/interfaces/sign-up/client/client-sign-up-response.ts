export interface ClientSignUpResponse {
  id: string;
  role: string;
  email: string;
  isEmailConfirmed: boolean;
  phoneNumber: string;
}