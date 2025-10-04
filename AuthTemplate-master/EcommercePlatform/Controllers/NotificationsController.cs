using Azure;
using CloudinaryDotNet.Actions;
using Ecommerce.DataAccess.Services.Notifications;
using Ecommerce.Entities.DTO.Notification;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Google.Apis.Requests.BatchRequest;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<User> _userManager;

        public NotificationsController(INotificationService notificationService, UserManager<User> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        [HttpGet("user/notification")]
        [Authorize(Roles="Client")]
        public async Task<IActionResult> GetMyNotifications(bool onlyUnread = false)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _notificationService.GetUserNotificationsAsync(userId, onlyUnread);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("user/mark-read/{id}")]
        [Authorize(Roles="Admin,Client")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {

            var response = await _notificationService.MarkAsReadAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("admin/notification")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminNotifications(bool onlyUnread = false)
        {
            var response = await _notificationService.GetAdminNotificationsAsync(onlyUnread);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
