using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecommerce.Entities.Models;
using System.Threading.Tasks;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.Notifications
{
    public interface INotificationService
    {
        Task NotifyUserAsync(string recipientId, string senderId, string title, string message);
        Task <Response<string>> MarkAsReadAsync(Guid notificationId);
        Task<Response<List<Notification>>> GetUserNotificationsAsync(string recipientId, bool onlyUnread = false);
        Task<Response<List<Notification>>> GetAdminNotificationsAsync(bool onlyUnread = false);
    }

}
