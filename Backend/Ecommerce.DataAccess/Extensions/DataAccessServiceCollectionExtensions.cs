using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Jobs;
using Ecommerce.DataAccess.Services.Auth;
using Ecommerce.DataAccess.Services.Cart;
using Ecommerce.DataAccess.Services.DebrisAlert;
using Ecommerce.DataAccess.Services.DebrisTracking;
using Ecommerce.DataAccess.Services.Email;
using Ecommerce.DataAccess.Services.ImageUploading;
using Ecommerce.DataAccess.Services.Notifications;
using Ecommerce.DataAccess.Services.OAuth;
using Ecommerce.DataAccess.Services.OrbitalPropagation;
using Ecommerce.DataAccess.Services.Order;
using Ecommerce.DataAccess.Services.OTP;
using Ecommerce.DataAccess.Services.Payment;
using Ecommerce.DataAccess.Services.Reviews;
using Ecommerce.DataAccess.Services.ServiceCatalog;
using Ecommerce.DataAccess.Services.ServiceCategory;
using Ecommerce.DataAccess.Services.Token;
using Ecommerce.Services.Reviews;
using Ecommerce.Utilities.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System.Net;
using System.Net.Mail;
using OrderService = Ecommerce.DataAccess.Services.Order.OrderService;
using ReviewService = Ecommerce.Services.Reviews.ReviewService;
//using OrderService = Ecommerce.DataAccess.Services.Order.OrderService;

namespace Ecommerce.DataAccess.Extensions
{
    public static class DataAccessServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("ProdCS")));

            return services;
        }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IImageUploadService, CloudinaryImageUploadService>();
            services.AddScoped<ITokenStoreService, TokenStoreService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IAuthGoogleService, AuthGoogleService>();
            services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
            //services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDebrisAlertService, DebrisAlertService>();
            services.AddScoped<IOrbitalPropagationService, OrbitalPropagationService>();
            services.AddScoped<IDebrisAlertJob, DebrisAlertJob>();
            return services;
        }

        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();

            services.AddFluentEmail(emailSettings.FromEmail)
                .AddSmtpSender(new SmtpClient(emailSettings.SmtpServer)
                {
                    Port = emailSettings.SmtpPort,
                    Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
                    EnableSsl = emailSettings.EnableSsl,
                    UseDefaultCredentials = false,
                });

            return services;
        }


        public static IServiceCollection AddStripeConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var stripeSettings = configuration.GetSection("Stripe").Get<StripeSettings>();

            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = stripeSettings.SecretKey;
            return services;
        }
    }
}
