using Ecommerce.Entities.DTO.Account.Auth.Login;

using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Email is required.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Email must be valid (e.g., user@example.com).");

           
            RuleFor(x => x.Otp)
                .Length(6).When(x => !string.IsNullOrEmpty(x.Otp))
                .WithMessage("OTP must be 6 characters long.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches(@"[!@#$%^&*]").WithMessage("Password must contain at least one special character (!@#$%^&*).");


        }
    }
}
