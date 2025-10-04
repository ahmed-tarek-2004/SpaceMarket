using Ecommerce.Entities.DTO.ServiceCatalog;
using Ecommerce.Utilities.Enums;
using FluentValidation;

namespace Ecommerce.API.Validators.ServiceCatalog
{
    public class UpdateServiceStatusRequestValidator : AbstractValidator<UpdateServiceStatusRequest>
    {
        public UpdateServiceStatusRequestValidator()
        {
            RuleFor(x => x.ServiceId)
                .NotEmpty().WithMessage("ServiceId is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => Enum.TryParse<ServiceStatus>(status, true, out _))
                .WithMessage("Invalid status value. Allowed: PendingApproval, Active, Suspended.");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason must be less than 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Reason));
        }
    }
}
