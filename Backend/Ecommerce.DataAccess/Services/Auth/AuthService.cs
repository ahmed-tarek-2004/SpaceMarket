using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.Email;
using Ecommerce.DataAccess.Services.ImageUploading;
using Ecommerce.DataAccess.Services.OTP;
using Ecommerce.DataAccess.Services.Token;
using Ecommerce.Entities.DTO.Account.Auth;
using Ecommerce.Entities.DTO.Account.Auth.Login;
using Ecommerce.Entities.DTO.Account.Auth.Register;
using Ecommerce.Entities.DTO.Account.Auth.ResetPassword;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Models.Auth.Users;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using LoginRequest = Ecommerce.Entities.DTO.Account.Auth.Login.LoginRequest;
using ResetPasswordRequest = Ecommerce.Entities.DTO.Account.Auth.ResetPassword.ResetPasswordRequest;


namespace Ecommerce.DataAccess.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IOTPService _otpService;
        private readonly ResponseHandler _responseHandler;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly ILogger<AuthService> _logger;
        private readonly IImageUploadService _imageUploadService;

        public AuthService(UserManager<User> userManager, ApplicationDbContext context, IEmailService emailService, IOTPService otpService, ResponseHandler responseHandler, ITokenStoreService tokenStoreService, ILogger<AuthService> logger, IImageUploadService imageUploadService)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _otpService = otpService;
            _responseHandler = responseHandler;
            _tokenStoreService = tokenStoreService;
            _logger = logger;
            _imageUploadService = imageUploadService;
        }

        public async Task<Response<LoginResponse>> LoginAsync(LoginRequest loginRequest)
        {
            // Find user by email or phone number
            User? user = await FindUserByEmailOrPhoneAsync(loginRequest.Email);

            if (user == null)
                return _responseHandler.NotFound<LoginResponse>("User not found.");

            // Check password
            if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return _responseHandler.BadRequest<LoginResponse>("Invalid password.");

            // Check if email is confirmed
            if (!user.EmailConfirmed)
                return _responseHandler.BadRequest<LoginResponse>("Email is not verified. Please verify your email first.");

            // If OTP is not provided, generate and send OTP
            if (string.IsNullOrEmpty(loginRequest.Otp))
            {
                var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);
                await _emailService.SendOtpEmailAsync(user, otp);
                _logger.LogInformation($"Otp Sent is : {otp}");

                var otpResponse = new LoginResponse { Id = user.Id };

                return _responseHandler.Success(
                    otpResponse,
                    "OTP sent to your email. Please provide the OTP to complete login."
                );
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);


            // Verify OTP
            var isOtpValid = await _otpService.ValidateOtpAsync(user.Id, loginRequest.Otp);
            if (!isOtpValid)
                return _responseHandler.BadRequest<LoginResponse>("Invalid or expired OTP.");

            // Generate tokens
            var tokens = await _tokenStoreService.GenerateAndStoreTokensAsync(user.Id, user);

            // Get display name based on role
            string displayName = await GetUserDisplayNameAsync(user.Id, roles.FirstOrDefault());

            var response = new LoginResponse
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault(),
                IsEmailConfirmed = user.EmailConfirmed,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                DisplayName = displayName
            };

            return _responseHandler.Success(response, "Login successful.");
        }
        public async Task<Response<RegisterResponse>> RegisterAsync(RegisterRequest registerRequest)
        {
            _logger.LogInformation("RegisterAsync started for Email: {Email}", registerRequest.Email);

            var emailPhoneCheck = await CheckIfEmailOrPhoneExists(registerRequest.Email, registerRequest.PhoneNumber);
            if (emailPhoneCheck != null)
            {
                _logger.LogWarning("Registration failed: {Reason}", emailPhoneCheck);
                return _responseHandler.BadRequest<RegisterResponse>(emailPhoneCheck);
            }

            // Create Identity User
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = registerRequest.Email, // Modify it by what you need
                    Email = registerRequest.Email,
                    PhoneNumber = registerRequest.PhoneNumber,
                };

                var createUserResult = await _userManager.CreateAsync(user, registerRequest.Password);
                if (!createUserResult.Succeeded)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description).ToList();
                    _logger.LogWarning("User creation failed for Email: {Email}. Errors: {Errors}", registerRequest.Email, string.Join(", ", errors));
                    return _responseHandler.BadRequest<RegisterResponse>(string.Join(", ", errors));
                }

                // Assign User role
                await _userManager.AddToRoleAsync(user, "USER");
                _logger.LogInformation("User created and role 'User' assigned. ID: {UserId}", user.Id);

                //await _userManager.CreateAsync(user);

                var tokens = await _tokenStoreService.GenerateAndStoreTokensAsync(user.Id, user);

                var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);

                // Send OTP via Email
                await _emailService.SendOtpEmailAsync(user, otp);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("User registration completed successfully. Email sent to {Email}", registerRequest.Email);


                var response = new RegisterResponse
                {
                    Email = registerRequest.Email,
                    Id = user.Id,
                    IsEmailConfirmed = false,
                    PhoneNumber = registerRequest.PhoneNumber,
                    Role = "USER",
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken
                };

                return _responseHandler.Created<RegisterResponse>(response, "User registered successfully. Please check your email to receive the OTP.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred during RegisterUserAsync for Email: {Email}", registerRequest.Email);
                return _responseHandler.BadRequest<RegisterResponse>("An error occurred during registration.");
            }
        }
        public async Task<Response<ClientRegisterResponse>> RegisterAsClientAsync(ClientRegisterRequest clientregisterRequest)
        {
            _logger.LogInformation("RegisterAsync started for Email: {Email}", clientregisterRequest.Email);

            var emailPhoneCheck = await CheckIfEmailOrPhoneExists(clientregisterRequest.Email, clientregisterRequest.PhoneNumber);
            if (emailPhoneCheck != null)
            {
                _logger.LogWarning("Registration failed: {Reason}", emailPhoneCheck);
                return _responseHandler.BadRequest<ClientRegisterResponse>(emailPhoneCheck);
            }

            // Create Identity User
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = clientregisterRequest.Email, // Modify it by what you need
                    Email = clientregisterRequest.Email,
                    PhoneNumber = clientregisterRequest.PhoneNumber,
                };

                var createUserResult = await _userManager.CreateAsync(user, clientregisterRequest.Password);
                if (!createUserResult.Succeeded)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description).ToList();
                    _logger.LogWarning("User creation failed for Email: {Email}. Errors: {Errors}", clientregisterRequest.Email, string.Join(", ", errors));
                    return _responseHandler.BadRequest<ClientRegisterResponse>(string.Join(", ", errors));
                }

                // Assign User role
                await _userManager.AddToRoleAsync(user, "Client");
                _logger.LogInformation("User created and role 'Client' assigned. ID: {UserId}", user.Id);

                //                await _userManager.CreateAsync(user);
                var client = new Client()
                {
                    User = user,
                    Address = clientregisterRequest.Country,
                    FullName = clientregisterRequest.FullName,
                    Organization = clientregisterRequest.OrganizationName
                };
                var createdClientResult = await _context.Clients.AddAsync(client);
                // await _context.SaveChangesAsync();
                _logger.LogInformation($"Client Adding Result is : {createdClientResult.State}");


                // var tokens = await _tokenStoreService.GenerateAndStoreTokensAsync(user.Id, user);

                var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);

                // Send OTP via Email
                await _emailService.SendOtpEmailAsync(user, otp);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("User registration completed successfully. Email sent to {Email}", clientregisterRequest.Email);


                var response = new ClientRegisterResponse
                {
                    Email = clientregisterRequest.Email,
                    Id = user.Id,
                    isEmailConfirmed = false,
                    PhoneNumber = clientregisterRequest.PhoneNumber,
                    Role = "Client",
                };

                return _responseHandler.Created<ClientRegisterResponse>(response, "Client registered successfully. Please check your email to receive the OTP.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred during ClientRegisterUserAsync for Email: {Email}", clientregisterRequest.Email);
                return _responseHandler.BadRequest<ClientRegisterResponse>("An error occurred during registration.");
            }
        }



        public async Task<Response<RegisterServiceProviderResponse>> RegisterProviderAsync(RegisterServiceProviderRequest registerRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering new service provider with Email: {Email}", registerRequest.Email);

            var emailPhoneCheck = await CheckIfEmailOrPhoneExists(registerRequest.Email, registerRequest.PhoneNumber);
            if (emailPhoneCheck != null)
            {
                _logger.LogWarning("Registration failed: {Reason}", emailPhoneCheck);
                return _responseHandler.BadRequest<RegisterServiceProviderResponse>(emailPhoneCheck);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var identityUser = new User
                {
                    UserName = registerRequest.Email,
                    Email = registerRequest.Email,
                    PhoneNumber = registerRequest.PhoneNumber
                };

                var createUserResult = await _userManager.CreateAsync(identityUser, registerRequest.Password);
                if (!createUserResult.Succeeded)
                {
                    var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("User creation failed for Email: {Email}. Errors: {Errors}", registerRequest.Email, errors);
                    return _responseHandler.BadRequest<RegisterServiceProviderResponse>(errors);
                }

                // Assign role
                await _userManager.AddToRoleAsync(identityUser, "SERVICEPROVIDER");
                _logger.LogInformation("User created and role 'SERVICEPROVIDER' assigned. UserId: {UserId}", identityUser.Id);

                // Create ServiceProvider entity

                var certificationUrls = new List<string>();

                if (registerRequest.CertificationFiles != null && registerRequest.CertificationFiles.Any())
                {
                    foreach (var file in registerRequest.CertificationFiles)
                    {
                        var allowedImageTypes = new[] { "image/png", "image/jpg", "image/jpeg", "image/jfif" };
                        if (!allowedImageTypes.Contains(file.ContentType.ToLower()))
                        {
                            return _responseHandler.BadRequest<RegisterServiceProviderResponse>(
                                "Only image files (.png, .jpg, .jpeg, .jfif) are allowed for certifications."
                            );
                        }

                        var uploadedUrl = await _imageUploadService.UploadCertificateAsync(file, identityUser.Id);
                        certificationUrls.Add(uploadedUrl);
                    }
                }


                var serviceProvider = new ServiceProvider
                {
                    Id = identityUser.Id,
                    User = identityUser,
                    CompanyName = registerRequest.CompanyName,
                    WebsiteUrl = registerRequest.WebsiteUrl ?? string.Empty,
                    CertificationsUrls = certificationUrls,
                    Status = ServiceProviderStatus.PendingApproval,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.ServiceProviders.AddAsync(serviceProvider, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // Generate and send OTP
                var otp = await _otpService.GenerateAndStoreOtpAsync(identityUser.Id);
                await _emailService.SendOtpEmailAsync(identityUser, otp);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Service provider registration completed successfully. Email sent to {Email}", registerRequest.Email);

                var response = new RegisterServiceProviderResponse
                {
                    Id = identityUser.Id,
                    Email = identityUser.Email,
                    PhoneNumber = identityUser.PhoneNumber,
                    CompanyName = serviceProvider.CompanyName,
                    WebsiteUrl = serviceProvider.WebsiteUrl,
                    CertificationsUrls = certificationUrls,
                    Role = "SERVICEPROVIDER",
                    Status = serviceProvider.Status.ToString()
                };

                return _responseHandler.Created(response, "Service provider registered successfully. Please check your email for the OTP.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred during RegisterProviderAsync for Email: {Email}", registerRequest.Email);
                return _responseHandler.BadRequest<RegisterServiceProviderResponse>("An error occurred during registration.");
            }
        }


        public async Task<Response<ForgetPasswordResponse>> ForgotPasswordAsync(ForgetPasswordRequest model)
        {
            _logger.LogInformation("Starting ForgotPasswordAsync for Email: {Email}, Phone: {Phone}", model.Email);

            // Find user by email or phone number
            User? user = await FindUserByEmailOrPhoneAsync(model.Email);

            if (user == null)
            {
                _logger.LogWarning("User not found for Email: {Email}, Phone: {Phone}", model.Email);
                return _responseHandler.NotFound<ForgetPasswordResponse>("User not found.");
            }

            // Generate and send OTP
            _logger.LogInformation("User found with ID: {UserId}. Generating OTP...", user.Id);
            var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);

            try
            {
                await _emailService.SendOtpEmailAsync(user, otp);
                _logger.LogInformation("OTP sent successfully to user ID: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to user ID: {UserId}", user.Id);
                //return _responseHandler.InternalServerError<ForgetPasswordResponse>("Failed to send OTP.");
            }
            var response = new ForgetPasswordResponse
            {
                UserId = user.Id
            };

            return _responseHandler.Success(response, "OTP sent to your email. Please use it to reset your password.");
        }
        public async Task<Response<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest model)
        {
            _logger.LogInformation("Starting ResetPasswordAsync for User ID: {UserId}", model.UserId);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", model.UserId);
                return _responseHandler.NotFound<ResetPasswordResponse>("User not found.");
            }

            // Verify OTP
            _logger.LogInformation("Validating OTP for User ID: {UserId}", user.Id);
            var isOtpValid = await _otpService.ValidateOtpAsync(model.UserId, model.Otp);
            if (!isOtpValid)
            {
                _logger.LogWarning("Invalid or expired OTP for User ID: {UserId}", model.UserId);
                return _responseHandler.BadRequest<ResetPasswordResponse>("Invalid or expired OTP.");

            }
            _logger.LogInformation("OTP is valid. Proceeding to reset password for User ID: {UserId}", user.Id);

            // Reset password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                _logger.LogWarning("Password reset failed for User ID: {UserId}. Errors: {Errors}", user.Id, string.Join(", ", errors));
                return _responseHandler.BadRequest<ResetPasswordResponse>(string.Join(", ", errors));
            }

            _logger.LogInformation("Password reset succeeded for User ID: {UserId}. Invalidating old tokens...", user.Id);

            // Invalidate all previous tokens for security
            await _tokenStoreService.InvalidateOldTokensAsync(user.Id);

            var roles = await _userManager.GetRolesAsync(user);
            var response = new ResetPasswordResponse
            {
                UserId = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "USER"
            };
            _logger.LogInformation("ResetPasswordAsync completed successfully for User ID: {UserId}", user.Id);

            return _responseHandler.Success(response, "Password reset successfully. Please log in with your new password.");
        }

        public async Task<Response<bool>> VerifyOtpAsync(VerifyOtpRequest verifyOtpRequest)
        {
            var user = await _userManager.FindByIdAsync(verifyOtpRequest.UserId);
            if (user == null)
                return _responseHandler.NotFound<bool>("User not found.");

            if (user.EmailConfirmed)
                return _responseHandler.Success(true, "Email is already verified.");

            var isOtpValid = await _otpService.ValidateOtpAsync(verifyOtpRequest.UserId, verifyOtpRequest.Otp);
            if (!isOtpValid)
                return _responseHandler.BadRequest<bool>("Invalid or expired OTP.");

            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return _responseHandler.BadRequest<bool>("Failed to update user confirmation status.");

            return _responseHandler.Success(true, "Email verified successfully.");
        }
        public async Task<Response<string>> ResendOtpAsync(ResendOtpRequest resendOtpRequest)
        {
            var user = await _userManager.FindByIdAsync(resendOtpRequest.UserId);
            if (user == null)
                return _responseHandler.NotFound<string>("User not found.");

            if (user.EmailConfirmed)
                return _responseHandler.Success<string>(null, "Email is already verified. No need to resend OTP.");

            var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);

            await _emailService.SendOtpEmailAsync(user, otp);

            return _responseHandler.Success<string>(null, "OTP resent successfully. Please check your email.");
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Starting RefreshTokenAsync for token: {TokenSnippet}", refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));

            try
            {
                var isValid = await _tokenStoreService.IsValidAsync(refreshToken);
                if (!isValid)
                {
                    _logger.LogWarning("Invalid refresh token provided: {TokenSnippet}", refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
                    throw new SecurityTokenException("Invalid refresh token");
                }

                var tokenEntry = await _context.UserRefreshTokens
                    .FirstOrDefaultAsync(r => r.Token == refreshToken);
                if (tokenEntry == null)
                {
                    _logger.LogWarning("No refresh token entry found for token: {TokenSnippet}", refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
                    throw new SecurityTokenException("Invalid refresh token");
                }

                var user = await _userManager.FindByIdAsync(tokenEntry.UserId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User not found for refresh token with UserId: {UserId}", tokenEntry.UserId);
                    throw new SecurityTokenException("Invalid user");
                }

                _logger.LogInformation("Invalidating old refresh tokens for user: {UserId}", user.Id);
                await _tokenStoreService.InvalidateOldTokensAsync(user.Id);

                _logger.LogInformation("Generating new access and refresh tokens for user: {UserId}", user.Id);
                var userTokens = await _tokenStoreService.GenerateAndStoreTokensAsync(user.Id, user);

                await _tokenStoreService.SaveRefreshTokenAsync(user.Id, userTokens.RefreshToken);
                _logger.LogInformation("New refresh token saved for user: {UserId}", user.Id);

                return new RefreshTokenResponse
                {
                    AccessToken = userTokens.AccessToken,
                    RefreshToken = userTokens.RefreshToken,
                };
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Security token error during refresh token process for token: {TokenSnippet}", refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during refresh token process for token: {TokenSnippet}", refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
                throw;
            }
        }

        // Helpers methods 
        private async Task<string?> CheckIfEmailOrPhoneExists(string email, string? phoneNumber)
        {
            if (await _userManager.FindByEmailAsync(email) != null)
                return "Email is already registered.";
            if (!string.IsNullOrEmpty(phoneNumber) && await _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber))
                return "Phone number is already registered.";
            return null;
        }
        private async Task<User?> FindUserByEmailOrPhoneAsync(string email)
        {
            if (!string.IsNullOrEmpty(email))
                return await _userManager.FindByEmailAsync(email);
            return null;
        }

        public async Task<Response<string>> LogoutAsync(ClaimsPrincipal userClaims)
        {
            try
            {
                // Get user ID from claims
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return _responseHandler.Unauthorized<string>("User not authenticated");
                }

                // Invalidate all refresh tokens for this user
                await _tokenStoreService.InvalidateOldTokensAsync(userId);

                return _responseHandler.Success<string>(null, "Logged out successfully");
            }
            catch (Exception ex)
            {
                return _responseHandler.ServerError<string>($"An error occurred during logout: {ex.Message}");
            }
        }

        public async Task<Response<string>> ChangePasswordAsync(ClaimsPrincipal userClaims, ChangePasswordRequest request)
        {
            try
            {
                // Get user ID from claims
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return _responseHandler.Unauthorized<string>("User not authenticated");
                }

                // Find user
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return _responseHandler.NotFound<string>("User not found");
                }

                // Verify current password
                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    return _responseHandler.BadRequest<string>("Current password is incorrect");
                }

                // Change password
                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return _responseHandler.BadRequest<string>(errors);
                }

                // Invalidate all existing refresh tokens for security
                await _tokenStoreService.InvalidateOldTokensAsync(userId);

                return _responseHandler.Success<string>(null, "Password changed successfully. Please login again.");
            }
            catch (Exception ex)
            {
                return _responseHandler.ServerError<string>($"An error occurred while changing password: {ex.Message}");
            }
        }

        private async Task<string> GetUserDisplayNameAsync(string userId, string role)
        {
            if (role == "Client")
            {
                var client = await _context.Clients.FindAsync(userId);
                return client?.FullName ?? string.Empty;
            }
            else if (role == "ServiceProvider")
            {
                var serviceProvider = await _context.ServiceProviders.FindAsync(userId);
                return serviceProvider?.CompanyName ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
