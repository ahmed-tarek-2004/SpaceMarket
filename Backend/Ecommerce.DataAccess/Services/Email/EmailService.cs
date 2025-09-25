using Ecommerce.Entities.Models.Auth.Identity;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Ecommerce.DataAccess.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IFluentEmail fluentEmail, UserManager<User> userManager, ILogger<EmailService> logger)
        {
            _fluentEmail = fluentEmail;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SendOtpEmailAsync(User user, string otp)
        {
            try
            {
                var rootPath = Directory.GetCurrentDirectory();
                var templatePath = Path.Combine(rootPath, "wwwroot", "EmailTemplates", "OtpVerificationEmail.html");

                if (!File.Exists(templatePath))
                {
                    _logger.LogError($"OTP Email Template not found at path: {templatePath}");
                    throw new FileNotFoundException("OTP Email Template not found.", templatePath);
                }

                var emailTemplate = await File.ReadAllTextAsync(templatePath);

                emailTemplate = emailTemplate
                    .Replace("{OtpCode}", otp)
                    .Replace("{CurrentYear}", DateTime.UtcNow.Year.ToString())
                    .Replace("{Username}", user.UserName ?? user.Email ?? "User");

                var sendResult = await _fluentEmail
                    .To(user.Email)
                    .Subject("Email Confirmation Code")
                    .Body(emailTemplate, isHtml: true)
                    .SendAsync();

                if (!sendResult.Successful)
                {
                    _logger.LogError($"Failed to send OTP email to {user.Email}. Errors: {string.Join(", ", sendResult.ErrorMessages)}");
                    throw new Exception("Failed to send OTP email.");
                }

                _logger.LogInformation($"OTP email successfully sent to {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while sending OTP email to {user.Email}");
                throw;
            }
        }
        public async Task SendServiceStatusChangedEmailAsync(User user, string serviceTitle, string newStatus, string? reason)
        {
            try
            {
                var rootPath = Directory.GetCurrentDirectory();
                var templatePath = Path.Combine(rootPath, "wwwroot", "EmailTemplates", "ServiceStatusChangedEmail.html");

                if (!File.Exists(templatePath))
                {
                    _logger.LogError($"ServiceStatusChanged Email Template not found at path: {templatePath}");
                    throw new FileNotFoundException("ServiceStatusChanged Email Template not found.", templatePath);
                }

                var emailTemplate = await File.ReadAllTextAsync(templatePath);

                emailTemplate = emailTemplate
                    .Replace("{ProviderName}", user.UserName ?? user.Email ?? "User")
                    .Replace("{ServiceTitle}", serviceTitle)
                    .Replace("{NewStatus}", newStatus)
                    .Replace("{Reason}", reason ?? "N/A")
                    .Replace("{PlatformName}", "Ecommerce Platform")
                    .Replace("{CurrentYear}", DateTime.UtcNow.Year.ToString());

                var sendResult = await _fluentEmail
                    .To(user.Email)
                    .Subject($"Your Service Status Changed to {newStatus}")
                    .Body(emailTemplate, isHtml: true)
                    .SendAsync();

                if (!sendResult.Successful)
                {
                    _logger.LogError($"Failed to send Service Status Changed email to {user.Email}. Errors: {string.Join(", ", sendResult.ErrorMessages)}");
                    throw new Exception("Failed to send Service Status Changed email.");
                }

                _logger.LogInformation($"Service status changed email successfully sent to {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while sending Service Status Changed email to {user.Email}");
                throw;
            }
        }

    }
}

