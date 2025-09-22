using System.Security.Claims;

using Ecommerce.Entities.DTO.Account.Auth;
using Ecommerce.Entities.DTO.Account.Auth.Login;
using Ecommerce.Entities.DTO.Account.Auth.Register;
using Ecommerce.Entities.DTO.Account.Auth.ResetPassword;
using Ecommerce.Entities.Shared.Bases;

using LoginRequest = Ecommerce.Entities.DTO.Account.Auth.Login.LoginRequest;
using ResetPasswordRequest = Ecommerce.Entities.DTO.Account.Auth.ResetPassword.ResetPasswordRequest;


namespace Ecommerce.DataAccess.Services.Auth
{
    public interface IAuthService
    {
        Task<Response<LoginResponse>> LoginAsync(LoginRequest loginRequest);
        Task<Response<RegisterResponse>> RegisterAsync(RegisterRequest registerRequest);
        Task<Response<bool>> VerifyOtpAsync(VerifyOtpRequest verifyOtpRequest);
        Task<Response<string>> ResendOtpAsync(ResendOtpRequest resendOtpRequest);
        Task<Response<ForgetPasswordResponse>> ForgotPasswordAsync(ForgetPasswordRequest model);
        Task<Response<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest model);
        Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken);
        Task<Response<string>> LogoutAsync(ClaimsPrincipal userClaims);
        Task<Response<string>> ChangePasswordAsync(ClaimsPrincipal user, ChangePasswordRequest request);
        Task<Response<ClientRegisterResponse>> RegisterAsClientAsync(ClientRegisterRequest clientregisterRequest);

    }
}
