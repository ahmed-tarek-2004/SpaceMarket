using Ecommerce.Entities.DTO.Reviews;
using FluentValidation;

namespace Ecommerce.API.Validators.Reviews
{
    public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
    {
        public CreateReviewRequestValidator()
        {
            RuleFor(x => x.ServiceId)
                .NotEmpty()
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("ServiceId must be a valid GUID.");
            RuleFor(x => x.ProviderId)
                .NotEmpty()
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("Provider must be a valid GUID.");
            RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        }
    }
}
