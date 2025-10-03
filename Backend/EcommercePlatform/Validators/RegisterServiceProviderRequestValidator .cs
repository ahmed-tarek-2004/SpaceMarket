using Ecommerce.Entities.DTO.Account.Auth.Register;
using FluentValidation;

namespace Ecommerce.API.Validators;

public class RegisterServiceProviderRequestValidator : AbstractValidator<RegisterServiceProviderRequest>
{
    private const int MaxFileSizeMB = 5;
    private const int MaxFileCount = 5;
    public RegisterServiceProviderRequestValidator()
    {

        RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Email must be valid (e.g., user@example.com).");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Phone number must contain only digits and be between 10 and 15 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[!@#$%^&*]").WithMessage("Password must contain at least one special character (!@#$%^&*).");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(200).WithMessage("Company name must be less than 200 characters.");

        RuleFor(x => x.WebsiteUrl)
            .MaximumLength(500).WithMessage("Website URL must be less than 500 characters.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Website URL must be a valid URL.")
            .When(x => !string.IsNullOrEmpty(x.WebsiteUrl));

        RuleFor(x => x.CertificationFiles)
               .NotNull().WithMessage("CertificateFile/s Must Not be Null")
               .NotEmpty().WithMessage("At least one certificate/image is required.")
               .Must(certs => certs != null && certs.Count <= MaxFileCount)
               .WithMessage($"A maximum of {MaxFileCount} certificates/images are allowed.");

        RuleForEach(x => x.CertificationFiles).ChildRules(cert =>
        {

            cert.RuleFor(f => f.Length)
                .LessThanOrEqualTo(MaxFileSizeMB * 1024 * 1024)
                .WithMessage($"Each certificate/image must be less than {MaxFileSizeMB} MB.");

            cert.RuleFor(f => f.ContentType)
                .Must(ct => new[] { "image/png", "image/jpg", "image/jpeg", "image/jfif" }
                                .Contains(ct.ToLower()))
                .WithMessage("Only image files (.png, .jpg, .jpeg, .jfif) are allowed.");

           
            
        });

    }
}
