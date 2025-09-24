using Ecommerce.Entities.DTO.ServiceCatalog;
using FluentValidation;

public class ServiceMetricsFilterRequestValidator : AbstractValidator<ServiceMetricsFilterRequest>
{
    public ServiceMetricsFilterRequestValidator()
    {
        // لو فيه EndDate
        // لازم يكون بعد StartDate
        When(x => x.StartDate.HasValue && x.EndDate.HasValue, () =>
        {
            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("EndDate must be after StartDate");
        });
    }
}
