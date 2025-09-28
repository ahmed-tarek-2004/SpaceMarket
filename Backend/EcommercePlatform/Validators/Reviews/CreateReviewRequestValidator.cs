using Ecommerce.Entities.DTO.Reviews;
using FluentValidation;

namespace Ecommerce.API.Validators.Reviews
{
    public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
    {
        public CreateReviewRequestValidator()
        {
            RuleFor(x => x.ServiceId).NotEmpty();
            RuleFor(x => x.ProviderId).NotEmpty();
            RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        }
    }
}
