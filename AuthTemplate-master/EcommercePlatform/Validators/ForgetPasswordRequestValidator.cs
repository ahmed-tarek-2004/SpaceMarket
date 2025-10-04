using Ecommerce.Entities.DTO.Account.Auth.ResetPassword;

using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
    {
        public ForgetPasswordRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Email is required.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Email must be valid (e.g., user@example.com).");


        }
    }
}
