using Ecommerce.Entities.DTO.Cart;
using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class UpdateCartItemValidator:AbstractValidator<UpdateCartItemRequest>
    {
        public UpdateCartItemValidator()
        {

            RuleFor(x => x.CartItemId)
                .NotEmpty().WithMessage("CartItem is required.");
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0);
        }

    }
}
