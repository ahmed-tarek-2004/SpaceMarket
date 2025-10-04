using Ecommerce.Entities.DTO.Reviews;
using FluentValidation;

namespace Ecommerce.API.Validators.Reviews
{
    public class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
    {
        public UpdateReviewRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("ServiceId must be a valid GUID."); ;
            // Rating/Text optional
        }
    }
}
