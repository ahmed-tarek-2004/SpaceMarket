using Ecommerce.Entities.DTO.Reviews;
using FluentValidation;

namespace Ecommerce.API.Validators.Reviews
{

    public class FilterReviewRequestValidator : AbstractValidator<ReviewFilterRequest>
    {
        public FilterReviewRequestValidator()
        {
            RuleFor(x => x.ServiceId)
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("ServiceId must be a valid GUID."); ;

            RuleFor(x => x.ProviderId)
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("ProviderId must be a valid GUID."); ;
            // Rating/Text optional
        }
    }
}
