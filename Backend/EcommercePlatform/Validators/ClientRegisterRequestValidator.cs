using Ecommerce.Entities.DTO.Account.Auth.Register;

using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class ClientRegisterRequestValidator : AbstractValidator<ClientRegisterRequest>
    {
        public ClientRegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be valid (e.g., user@example.com).");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Phone number must contain only digits and be between 10 and 15 characters.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MinimumLength(3).WithMessage("Full Name must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Full Name cannot exceed 50 characters.");
          
            RuleFor(x => x.OrganizationName)
                .NotEmpty().WithMessage("Organization Name is required.")
                .MinimumLength(3).WithMessage("Organization Name must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Organization Name cannot exceed 50 characters.");
            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches(@"[!@#$%^&*]").WithMessage("Password must contain at least one special character (!@#$%^&*).");
            
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Your Password")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required");
        }
    }
}
