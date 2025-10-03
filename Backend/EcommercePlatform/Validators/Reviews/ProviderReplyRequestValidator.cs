using Ecommerce.Entities.DTO.Reviews;
using FluentValidation;

namespace Ecommerce.API.Validators.Reviews
{
    public class ProviderReplyRequestValidator : AbstractValidator<ProviderReplyRequest>
    {
        public ProviderReplyRequestValidator()
        {
            RuleFor(x => x.ReviewId).NotEmpty();
            RuleFor(x => x.ReplyText).NotEmpty();
            RuleFor(x => x.ReviewId)
           .Must(id => Guid.TryParse(id, out _))
           .WithMessage("ProviderId must be a valid GUID.");

        }
    }
}
