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
                .Must(status => string.IsNullOrEmpty(status) || Enum.TryParse<ServiceStatus>(status, true, out _))
                .WithMessage("Invalid status value. Allowed: PendingApproval, Active, Suspended.");

        }
    }
}
