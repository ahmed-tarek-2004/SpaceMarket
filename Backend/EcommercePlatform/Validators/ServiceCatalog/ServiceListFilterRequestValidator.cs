using Ecommerce.Entities.DTO.ServiceCatalog;
using Ecommerce.Utilities.Enums;
using FluentValidation;

namespace Ecommerce.API.Validators.ServiceCatalog
{
    public class ServiceListFilterRequestValidator : AbstractValidator<ServiceListFilterRequest>
    {
        public ServiceListFilterRequestValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => Enum.TryParse<ServiceStatus>(status, true, out _))
                .WithMessage("Invalid status value. Allowed: PendingApproval, Active, Suspended.");
        }
    }
}
