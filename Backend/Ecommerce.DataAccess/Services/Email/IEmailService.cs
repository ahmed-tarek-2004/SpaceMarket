using Ecommerce.Entities.Models.Auth.Identity;

namespace Ecommerce.DataAccess.Services.Email
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(User applicationUser, string otp);
        Task SendServiceStatusChangedEmailAsync(User user, string serviceTitle, string newStatus, string? reason);
    }
}
