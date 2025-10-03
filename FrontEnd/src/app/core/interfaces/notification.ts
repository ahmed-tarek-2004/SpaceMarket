export interface INotificationResponse {
  id: string;
  recipientId: string;
  senderId: string;
  message: string;
  createdAt: string;
  isRead: boolean;
  title: string;
}
