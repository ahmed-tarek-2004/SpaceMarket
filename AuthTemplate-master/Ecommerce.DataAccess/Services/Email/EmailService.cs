using Ecommerce.Entities.Models.Auth.Identity;
using FluentEmail.Core;
using Google.Apis.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Ecommerce.Entities.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Stripe.Checkout;
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
            QuestPDF.Settings.License = LicenseType.Community;
        }

        #region SendOtpEmailAsync 
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
        #endregion 

        #region SendServiceStatusChangedEmailAsync
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
        #endregion

        #region SendPaymentReceiptAsync
        public async Task SendPaymentReceiptAsync(User user, Transaction transaction, Session session)
        {
            try
            {
                _logger.LogInformation("Start Sending Email To Client & Provider");
                var rootPath = Directory.GetCurrentDirectory();
                var templatePath = Path.Combine(rootPath, "wwwroot", "EmailTemplates", "CheckoutServiceSession.html");

                if (!File.Exists(templatePath))
                {
                    _logger.LogError($"ServiceStatusChanged Email Template not found at path: {templatePath}");
                    throw new FileNotFoundException("ServiceStatusChanged Email Template not found.", templatePath);
                }

                var emailTemplate = await File.ReadAllTextAsync(templatePath);

                emailTemplate = emailTemplate
                    .Replace("{ClientName}", user.UserName ?? "User")
                    .Replace("{ClientId}", user.Id)
                    .Replace("{ClientEmail}", user.Email)
                    .Replace("{ClientPhone}", user.PhoneNumber ?? "N/A")
                    .Replace("{ReceiptNumber}", Guid.NewGuid().ToString())
                    .Replace("{TransactionDate}", transaction.Date.ToString("yyyy/MM"))
                    .Replace("{TransactionTime}", transaction.Date.TimeOfDay.ToString())
                    .Replace("{PlatformName}", "Space Market Platform")
                    .Replace("{TotalAmount}", session.AmountTotal.ToString())
                    .Replace("{TotalSalary}", session.AmountTotal.ToString())
                    .Replace("{PaymentMethod}", session.PaymentMethodTypes.FirstOrDefault() ?? "credit Card")
                    .Replace("{TransactionId}", transaction.Id.ToString())
                    .Replace("{PaymentStatus}", session.PaymentStatus)
                    .Replace("{CurrentYear}", DateTime.UtcNow.Year.ToString());



                var pdfBytes = GenerateReceiptPdf(user, transaction, session);

                var attachment = new FluentEmail.Core.Models.Attachment
                {
                    Data = new MemoryStream(pdfBytes),
                    Filename = "Receipt.pdf",
                    ContentType = "application/pdf"
                };


                var sendResult = await _fluentEmail
                    .To(user.Email)
                    .Subject($"Your Service Receipt - {transaction.Status}")
                    .Body(emailTemplate, isHtml: true)
                    .Attach(attachment)
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
        #endregion

        #region GenerateReceiptPdf
        private byte[] GenerateReceiptPdf(User user, Transaction transaction, Session session)
        {
           
            var receiptNumber = Guid.NewGuid().ToString();
            var transactionDate = transaction.Date.ToString("yyyy/MM/dd");
            var transactionTime = transaction.Date.ToString("HH:mm:ss");
            var paymentMethod = session.PaymentMethodTypes.FirstOrDefault() ?? "Credit Card";
            var totalAmount = session.AmountTotal / 100m; // assuming cents
            var totalSalary = session.AmountTotal / 100m;
            var paymentStatus = session.PaymentStatus;

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontColor(Colors.Black));

                    // Header
                    page.Header()
                        .AlignCenter()
                        .Text("Space Market Platform")
                        .SemiBold()
                        .FontSize(20)
                        .FontColor(Colors.Blue.Medium);

                    // Footer
                    page.Footer()
                        .AlignCenter()
                        .Text("Thank you for your payment!")
                        .FontSize(12)
                        .FontColor(Colors.Grey.Darken1);

                    // Main content (ONE Content() call only)
                    page.Content().Column(col =>
                    {
                        // Horizontal line under header with padding
                        col.Item()
                            .PaddingBottom(10)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);

                        // Transaction & Client Info
                        col.Item().Text($"Receipt Number: {receiptNumber}").SemiBold();
                        col.Item().Text($"Transaction Date: {transactionDate}");
                        col.Item().Text($"Transaction Time: {transactionTime}");
                        col.Item().Text($"Client: {user.UserName ?? "User"} ({user.Email})");
                        col.Item().Text($"Client ID: {user.Id}");
                        col.Item().Text($"Client Phone: {user.PhoneNumber ?? "N/A"}");

                        col.Item().PaddingTop(10);

                        // Table with transaction details
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(5);
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Field").SemiBold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Value").SemiBold();
                            });

                            // Data rows
                            table.Cell().Padding(5).Text("Transaction ID");
                            table.Cell().Padding(5).Text(transaction.Id.ToString());

                            table.Cell().Padding(5).Text("Payment Method");
                            table.Cell().Padding(5).Text(paymentMethod);

                            table.Cell().Padding(5).Text("Total Amount");
                            table.Cell().Padding(5).Text($"{totalAmount:C}");

                            table.Cell().Padding(5).Text("Total Salary");
                            table.Cell().Padding(5).Text($"{totalSalary:C}");

                            table.Cell().Padding(5).Text("Payment Status");
                            table.Cell().Padding(5).Text(paymentStatus);
                        });

                        // Year at the bottom
                        col.Item().PaddingTop(10)
                            .Text($"Year: {DateTime.UtcNow.Year}")
                            .AlignRight();
                    });
                });
            })
            .GeneratePdf(); 

            return pdfBytes;
        }

        #endregion

    }
}
