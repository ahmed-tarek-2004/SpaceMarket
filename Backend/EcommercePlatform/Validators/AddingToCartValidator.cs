using Ecommerce.Entities.DTO.Cart;
using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class AddingToCartValidator:AbstractValidator<AddingToCartRequest>
    {
        public AddingToCartValidator()
        {
            RuleFor(x => x.ServiceId)
                .NotEmpty().WithMessage("Service is required.");
            RuleFor(x => x.DataSetId)
                .NotEmpty().WithMessage("Service is required.");
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
