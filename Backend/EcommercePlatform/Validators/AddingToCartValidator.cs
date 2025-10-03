using Ecommerce.Entities.DTO.Cart;
using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class AddingToCartRequestValidator : AbstractValidator<AddingToCartRequest>
    {
        public AddingToCartRequestValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .NotEmpty()
                .Must(x => (x.ServiceId.HasValue ^ x.DataSetId.HasValue)) // xor: exactly one provided
                .WithMessage("Provide exactly one of serviceId or dataSetId.");

            When(x => x.ServiceId.HasValue, () =>
            {
                RuleFor(x => x.ServiceId.Value).NotEqual(Guid.Empty).WithMessage("serviceId must be a valid guid.");
            });

            When(x => x.DataSetId.HasValue, () =>
            {
                RuleFor(x => x.DataSetId.Value).NotEqual(Guid.Empty).WithMessage("dataSetId must be a valid guid.");
            });
        }
    }
}
