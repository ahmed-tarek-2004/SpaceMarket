using Ecommerce.API.Extensions;
using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Extensions;
using Ecommerce.DataAccess.Seeder;
using Ecommerce.DataAccess.Services.Notifications;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Configurations;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

using StackExchange.Redis;
using System.Text.Json.Serialization;

namespace EcommercePlatform
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Host.UseSerilogLogging();

            // Active Model State
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(
                options => options.SuppressModelStateInvalidFilter = true
            );

            // IOptional Pattern
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("Authorization:Google"));
            builder.Services.Configure<CommissionSettings>(builder.Configuration.GetSection("CommissionSettings"));
            //builder.Services.Configure<UploadcareSettings>(builder.Configuration.GetSection("Uploadcare"));

            builder.Services.AddApplicationServices();
            builder.Services.AddScoped<ResponseHandler>();
            builder.Services.AddDatabase(builder.Configuration);
            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
            builder.Services.AddEmailServices(builder.Configuration);
            builder.Services.AddStripeConfiguration(builder.Configuration);
            builder.Services.AddSignalR();

            // Enum to string converter
            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));



            builder.Services.AddFluentValidation();

            // Rate limiter for otp resend
            builder.Services.AddResendOtpRateLimiter();

            // Add CORS services
            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAngularApp",
            //        policy =>
            //        {
            //            policy.WithOrigins("http://localhost:4200")
            //                  .AllowAnyHeader()
            //                  .AllowAnyMethod()
            //                  .AllowCredentials();
            //        });
            //});


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    policy =>
                    {
                        policy.AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials()
                              .SetIsOriginAllowed(_ => true); 
                    });
            });

            builder.Services.AddDataProtection()
                .PersistKeysToDbContext<ApplicationDbContext>()
                .SetApplicationName("AuthStarter");

            // For redis 
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"));
                configuration.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(configuration);
            });

            builder.Services.AddSwagger();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            #region Seed User,Role Data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<Ecommerce.Entities.Models.Auth.Identity.Role>>();

                await RoleSeeder.SeedAsync(roleManager);
                await UserSeeder.SeedAsync(userManager);
            }
            #endregion

            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAngularApp");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<NotificationHub>("/hub/notifications");

            app.MapControllers();

            app.Run();
        }
    }
}