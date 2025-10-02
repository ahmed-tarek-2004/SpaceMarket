using Ecommerce.Entities.DTO.Order;
using FluentValidation;

namespace Ecommerce.API.Validators.Order;

public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemRequestValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("ItemId is required.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid item type.");

        RuleFor(x => x.PriceSnapshot)
            .GreaterThan(0).WithMessage("PriceSnapshot must be greater than zero.");
    }
}