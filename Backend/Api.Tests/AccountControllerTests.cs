using System.Net;
using System.Security.Claims;

using Ecommerce.API.Controllers;
using Ecommerce.DataAccess.Services.Auth;
using Ecommerce.DataAccess.Services.OAuth;
using Ecommerce.Entities.DTO.Account.Auth;
using Ecommerce.Entities.DTO.Account.Auth.Login;
using Ecommerce.Entities.DTO.Account.Auth.Register;
using Ecommerce.Entities.DTO.Account.Auth.ResetPassword;
using Ecommerce.Entities.Shared.Bases;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

using Microsoft.IdentityModel.Tokens;

using Moq;

namespace Api.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly ResponseHandler _responseHandler;
        private readonly Mock<IValidator<RegisterRequest>> _mockRegisterValidator;
        private readonly Mock<IValidator<LoginRequest>> _mockLoginValidator;
        private readonly Mock<IValidator<ForgetPasswordRequest>> _mockForgetPasswordValidator;
        private readonly Mock<IValidator<ResetPasswordRequest>> _mockResetPasswordValidator;
        private readonly Mock<IValidator<ChangePasswordRequest>> _mockChangePasswordValidator;
        private readonly Mock<IAuthGoogleService> _mockAuthGoogleService;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockRegisterValidator = new Mock<IValidator<RegisterRequest>>();
            _mockLoginValidator = new Mock<IValidator<LoginRequest>>();
            _mockForgetPasswordValidator = new Mock<IValidator<ForgetPasswordRequest>>();
            _mockResetPasswordValidator = new Mock<IValidator<ResetPasswordRequest>>();
            _mockChangePasswordValidator = new Mock<IValidator<ChangePasswordRequest>>();
            _mockAuthGoogleService = new Mock<IAuthGoogleService>();
            _responseHandler = new ResponseHandler();

            _controller = new AccountController(
                _mockAuthService.Object,
                _responseHandler,
                _mockRegisterValidator.Object,
                _mockLoginValidator.Object,
                _mockForgetPasswordValidator.Object,
                _mockResetPasswordValidator.Object,
                _mockAuthGoogleService.Object,
                _mockChangePasswordValidator.Object);
            // Setup common response handler mocks
        }
     

        #region Login Tests

        [Fact]
        public async Task Login_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "test@test.com", Password = "Password123!" };
            var validationResult = new ValidationResult();
            var loginResponse = new Response<LoginResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Data = new LoginResponse { AccessToken = "token", RefreshToken = "refresh" }
            };

            _mockLoginValidator.Setup(v => v.ValidateAsync(loginRequest, default))
                .ReturnsAsync(validationResult);
            _mockAuthService.Setup(s => s.LoginAsync(loginRequest))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task Login_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "invalid", Password = "" };
            var validationResult = new ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Email", "Invalid email format"),
                new FluentValidation.Results.ValidationFailure("Password", "Password is required")
            });

            _mockLoginValidator.Setup(v => v.ValidateAsync(loginRequest, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        [Fact]
        public async Task Login_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "test@test.com", Password = "Password123!" };
            var validationResult = new ValidationResult();

            _mockLoginValidator.Setup(v => v.ValidateAsync(loginRequest, default))
                .ReturnsAsync(validationResult);
            _mockAuthService.Setup(s => s.LoginAsync(loginRequest))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.Login(loginRequest));
        }

        #endregion

        #region Register Tests

        [Fact]
        public async Task Register_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "test@test.com",
                Password = "Password123!",
                FirstName = "test",
                LastName = "usr",
                PhoneNumber = "01224309198"
            };
            var validationResult = new ValidationResult();
            var registerResponse = new Response<RegisterResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Data = new RegisterResponse { Id = "123", Email = "test@test.com" }
            };

            _mockRegisterValidator.Setup(v => v.ValidateAsync(registerRequest, default))
                .ReturnsAsync(validationResult);
            _mockAuthService.Setup(s => s.RegisterAsync(registerRequest))
                .ReturnsAsync(registerResponse);

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task Register_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequest { Email = "invalid-email" };
            var validationResult = new ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Email", "Invalid email format"),
                new FluentValidation.Results.ValidationFailure("Password", "Password is required")
            });

            _mockRegisterValidator.Setup(v => v.ValidateAsync(registerRequest, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        #endregion

        #region Verify OTP Tests

        [Fact]
        public async Task VerifyOtp_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var verifyOtpRequest = new VerifyOtpRequest { UserId = "123", Otp = "123456" };
            var response = new Response<bool> { StatusCode = HttpStatusCode.OK, Data = true };

            _controller.ModelState.Clear();
            _mockAuthService.Setup(s => s.VerifyOtpAsync(verifyOtpRequest))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.VerifyOtp(verifyOtpRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task VerifyOtp_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var verifyOtpRequest = new VerifyOtpRequest();
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.VerifyOtp(verifyOtpRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        #endregion

        #region Resend OTP Tests

        [Fact]
        public async Task ResendOtp_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var resendOtpRequest = new ResendOtpRequest { UserId = "123" };
            var response = new Response<string> { StatusCode = HttpStatusCode.OK, Data = "OTP sent successfully" };

            _controller.ModelState.Clear();
            _mockAuthService.Setup(s => s.ResendOtpAsync(resendOtpRequest))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ResendOtp(resendOtpRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        #endregion

        #region Forget Password Tests

        [Fact]
        public async Task ForgetPassword_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var forgetPasswordRequest = new ForgetPasswordRequest { Email = "test@test.com" };
            var validationResult = new ValidationResult();
            var response = new Response<ForgetPasswordResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Data = new ForgetPasswordResponse { UserId = "123" }
            };

            _mockForgetPasswordValidator.Setup(v => v.ValidateAsync(forgetPasswordRequest, default))
                .ReturnsAsync(validationResult);
            _mockAuthService.Setup(s => s.ForgotPasswordAsync(forgetPasswordRequest))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ForgetPassword(forgetPasswordRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task ForgetPassword_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var forgetPasswordRequest = new ForgetPasswordRequest { Email = "invalid-email" };
            var validationResult = new ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Email", "Invalid email format")
            });

            _mockForgetPasswordValidator.Setup(v => v.ValidateAsync(forgetPasswordRequest, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.ForgetPassword(forgetPasswordRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        #endregion

        #region Reset Password Tests

        [Fact]
        public async Task ResetPassword_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var resetPasswordRequest = new ResetPasswordRequest
            {
                UserId = "123",
                Otp = "123456",
                NewPassword = "NewPassword123!",
                ConfirmPassword = "NewPassword123"
            };
            var validationResult = new ValidationResult();
            var response = new Response<ResetPasswordResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Data = new ResetPasswordResponse { UserId = "123" }
            };

            _mockResetPasswordValidator.Setup(v => v.ValidateAsync(resetPasswordRequest, default))
                .ReturnsAsync(validationResult);
            _mockAuthService.Setup(s => s.ResetPasswordAsync(resetPasswordRequest))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ResetPassword(resetPasswordRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task ResetPassword_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var resetPasswordRequest = new ResetPasswordRequest { UserId = "123" };
            var validationResult = new ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Email", "Invalid email format"),
                new FluentValidation.Results.ValidationFailure("Token", "Token is required"),
                new FluentValidation.Results.ValidationFailure("NewPassword", "Password is required")
            });

            _mockResetPasswordValidator.Setup(v => v.ValidateAsync(resetPasswordRequest, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.ResetPassword(resetPasswordRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        #endregion

        #region Refresh Token Tests

        [Fact]
        public async Task RefreshToken_ValidToken_ReturnsOkResult()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            var newTokens = new RefreshTokenResponse
            {
                AccessToken = "new-access-token",
                RefreshToken = "new-refresh-token"
            };

            _mockAuthService.Setup(s => s.RefreshTokenAsync(refreshToken))
                .ReturnsAsync(newTokens);

            // Act
            var result = await _controller.RefreshToken(refreshToken);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_EmptyToken_ReturnsBadRequest()
        {
            // Arrange
            var refreshToken = "";

            // Act
            var result = await _controller.RefreshToken(refreshToken);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var refreshToken = "invalid-refresh-token";

            _mockAuthService.Setup(s => s.RefreshTokenAsync(refreshToken))
                .ThrowsAsync(new SecurityTokenException("Invalid token"));

            // Act
            var result = await _controller.RefreshToken(refreshToken);

            // Assert
            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, actionResult.StatusCode);
        }

        #endregion

        #region Change Password Tests

        [Fact]
        public async Task ChangePassword_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var changePasswordRequest = new ChangePasswordRequest
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };
            var validationResult = new ValidationResult();
            var response = new Response<string>
            {
                StatusCode = HttpStatusCode.OK,
                Data = "Password changed successfully"
            };

            var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-id-123")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claims }
            };

            _mockChangePasswordValidator.Setup(v => v.ValidateAsync(changePasswordRequest, default))
                .ReturnsAsync(validationResult);
            _mockAuthService.Setup(s => s.ChangePasswordAsync(It.IsAny<ClaimsPrincipal>(), changePasswordRequest))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ChangePassword(changePasswordRequest);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var changePasswordRequest = new ChangePasswordRequest
            {
                CurrentPassword = "",
                NewPassword = "weak",
                ConfirmNewPassword = "different"
            };
            var validationResult = new ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("CurrentPassword", "Current password is required"),
                new FluentValidation.Results.ValidationFailure("NewPassword", "Password must be at least 8 characters long"),
                new FluentValidation.Results.ValidationFailure("ConfirmPassword", "Passwords do not match")
            });

            _mockChangePasswordValidator.Setup(v => v.ValidateAsync(changePasswordRequest, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.ChangePassword(changePasswordRequest);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task Logout_AuthenticatedUser_ReturnsOkResult()
        {
            // Arrange
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-id-123")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claims }
            };

            var response = new Response<string>
            {
                StatusCode = HttpStatusCode.OK,
                Data = "Logged out successfully"
            };

            _mockAuthService.Setup(s => s.LogoutAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Logout();

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task Logout_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var response = new Response<string>
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Message = "User not authenticated"
            };

            _mockAuthService.Setup(s => s.LogoutAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Logout();

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(401, actionResult.StatusCode);
        }

        #endregion
    }
}