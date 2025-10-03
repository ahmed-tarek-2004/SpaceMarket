using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.Notification;
using Ecommerce.Entities.DTO.Payment;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly ILogger<NotificationService> _logger;
        private readonly ResponseHandler _responseHandler;

        public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hub, ResponseHandler responseHandler, ILogger<NotificationService> logger)
        {
            _context = context;
            _hub = hub;
            _responseHandler = responseHandler;
            _logger = logger;
        }


        public async Task NotifyUserAsync(string recipientId, string senderId, string title, string message)
        {
            try
            {var notification = new Notification
            {
                RecipientId = recipientId,
                SenderId = senderId,
                Title = title,
                Message = message
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(recipientId))
                {

                    await _hub.Clients.Group(recipientId).SendAsync("ReceiveNotification", notification);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error When Adding Notification");
            }
        }

        public async Task<Response<string>> MarkAsReadAsync(Guid notificationId)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    await _context.SaveChangesAsync();
                }
                return _responseHandler.NotFound<string>($"notification with Id {notificationId} >>Not Found<<");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error When retrieving Notification");
                return _responseHandler.ServerError<string>("Failed to Retrieve Notification");
            }
        }

        public async Task<Response<List<Notification>>> GetUserNotificationsAsync(string recipientId, bool onlyUnread = false)
        {
            try
            {
                var query = _context.Notifications.AsQueryable()
                  .Where(n => n.RecipientId == recipientId);

                if (onlyUnread)
                    query = query.Where(n => !n.IsRead);

                var respone = await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
                return _responseHandler.Success<List<Notification>>(respone, "Notification Retrieved Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error When retrieving Notification");
                return _responseHandler.ServerError<List<Notification>>("Failed to Retrieve Notification");
            }
        }

        public async Task<Response<List<Notification>>> GetAdminNotificationsAsync(bool onlyUnread = false)
        {
            try
            {
                var query = _context.Notifications.AsQueryable();

                if (onlyUnread)
                    query = query.Where(n => !n.IsRead);

                var respone = await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
                return _responseHandler.Success<List<Notification>>(respone, "Notification Retrieved Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error When retrieving Notification");
                return _responseHandler.ServerError<List<Notification>>("Failed to Retrieve Notification");
            }
        }
    }

}
