using Ecommerce.DataAccess.Services.Auth;
using Ecommerce.DataAccess.Services.OAuth;
using Ecommerce.Entities.DTO.Account.Auth;
using Ecommerce.Entities.DTO.Account.Auth.Login;
using Ecommerce.Entities.DTO.Account.Auth.Register;
using Ecommerce.Entities.DTO.Account.Auth.ResetPassword;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Exceptions;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ResponseHandler _responseHandler;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<ClientRegisterRequest> _clientregisterValidator;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IValidator<ForgetPasswordRequest> _forgetPasswordValidator;
        private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;
        private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
        private readonly IAuthGoogleService _authGoogleService;


        public AccountController(IAuthService authService, ResponseHandler responseHandler, IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator, IValidator<ForgetPasswordRequest> forgetPasswordValidator, IValidator<ResetPasswordRequest> resetPasswordValidator, IAuthGoogleService authGoogleService, IValidator<ChangePasswordRequest> changePasswordValidator,IValidator<ClientRegisterRequest>clientRegisterValidator)
        {
            _authService = authService;
            _responseHandler = responseHandler;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _forgetPasswordValidator = forgetPasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
            _authGoogleService = authGoogleService;
            _changePasswordValidator = changePasswordValidator;
            _clientregisterValidator = clientRegisterValidator;
        }
        [HttpPost("login")]
        public async Task<ActionResult<Response<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            ValidationResult validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                    _responseHandler.BadRequest<object>(errors));
            }

            var response = await _authService.LoginAsync(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("login/google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest googleLoginDto)
        {
            if (!ModelState.IsValid)
                return _responseHandler.HandleModelStateErrors(ModelState);

            try
            {
                var token = await _authGoogleService.AuthenticateWithGoogleAsync(googleLoginDto.IdToken);
                var response = _responseHandler.Success(token, "Logged in with Google successfully");
                return StatusCode((int)response.StatusCode, response);
            }
            catch (UnauthorizedAccessException ex)  // When 'IdToke' is not valid
            {
                var response = _responseHandler.Unauthorized<string>("Google authentication failed: " + ex.Message);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (UserCreationException ex)  // When user creation faild
            {
                var response = _responseHandler.InternalServerError<string>("User creation failed: " + ex.Message);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)  // Server error
            {
                var response = _responseHandler.ServerError<string>("An error occurred: " + ex.Message);
                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<Response<RegisterResponse>>> Register([FromForm] RegisterRequest request)
        {
            ValidationResult validationResult = await _registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                    _responseHandler.BadRequest<object>(errors));
            }

            var response = await _authService.RegisterAsync(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("register/client")]
        public async Task<ActionResult<Response<RegisterResponse>>> RegisterAsBuyer([FromForm] ClientRegisterRequest request)
        {
            ValidationResult validationResult = await _clientregisterValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                    _responseHandler.BadRequest<object>(errors));
            }

            var response = await _authService.RegisterAsClientAsync(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<Response<bool>>> VerifyOtp([FromBody] VerifyOtpRequest model)
        {
            if (!ModelState.IsValid)
                return StatusCode((int)_responseHandler.BadRequest<object>("Invalid input data.").StatusCode,
                    _responseHandler.BadRequest<object>("Invalid input data."));

            var result = await _authService.VerifyOtpAsync(model);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("resend-otp")]
        [EnableRateLimiting("SendOtpPolicy")]
        public async Task<ActionResult<Response<string>>> ResendOtp([FromBody] ResendOtpRequest model)
        {
            if (!ModelState.IsValid)
                return StatusCode((int)_responseHandler.BadRequest<object>("Invalid input data.").StatusCode,
                    _responseHandler.BadRequest<object>("Invalid input data."));

            var result = await _authService.ResendOtpAsync(model);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost("forget-password")]
        public async Task<ActionResult<Response<ForgetPasswordResponse>>> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            ValidationResult validationResult = await _forgetPasswordValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                    _responseHandler.BadRequest<object>(errors));
            }

            var response = await _authService.ForgotPasswordAsync(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<Response<ResetPasswordResponse>>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            ValidationResult validationResult = await _resetPasswordValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                    _responseHandler.BadRequest<object>(errors));
            }

            var response = await _authService.ResetPasswordAsync(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest(_responseHandler.BadRequest<string>("RefreshTokenIsNotFound"));
            try
            {
                var newTokens = await _authService.RefreshTokenAsync(refreshToken);

                return Ok(_responseHandler.Success<RefreshTokenResponse>(newTokens, "User token refreshed succsessfully"));
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(_responseHandler.Unauthorized<string>(ex.Message));
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    _responseHandler.BadRequest<string>("UnexpectedError" + ": " + error)
                );
            }
        }
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var validationResult = await _changePasswordValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return BadRequest(_responseHandler.BadRequest<object>(errors));
            }

            var response = await _authService.ChangePasswordAsync(User, request);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var response = await _authService.LogoutAsync(User);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
