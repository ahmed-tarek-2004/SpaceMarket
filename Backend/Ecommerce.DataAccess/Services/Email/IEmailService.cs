using Ecommerce.Entities.Models.Auth.Identity;

namespace Ecommerce.DataAccess.Services.Email
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(User applicationUser, string otp);
    }
}
