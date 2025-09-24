using Ecommerce.Entities.DTO.ServiceCatalog;
using FluentValidation;

namespace Ecommerce.API.Validators.ServiceCatalog
{
    public class ServiceListFilterRequestValidator : AbstractValidator<ServiceListFilterRequest>
    {
        public ServiceListFilterRequestValidator()
        {
            RuleFor(x => x.Status)
                .Must(status => string.IsNullOrEmpty(status) ||
                                new[] { "Active", "PendingApproval", "Suspended" }
                                .Contains(status))
                .WithMessage("Invalid status filter.");
        }
    }
}
