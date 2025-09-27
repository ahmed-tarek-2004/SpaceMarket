using Ecommerce.Entities.DTO.ServiceCatalog;
using FluentValidation;

namespace Ecommerce.API.Validators.ServiceCatalog
{
    public class ServiceFilterRequestValidator:AbstractValidator<ServiceFilterRequest>
    {
        public ServiceFilterRequestValidator()
        {
            RuleFor(x => x.PageNumber)
           .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");
        }
    }
}
