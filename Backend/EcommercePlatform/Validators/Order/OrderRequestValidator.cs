using Ecommerce.Entities.DTO.Order;
using FluentValidation;

namespace Ecommerce.API.Validators.Order;

public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("ClientId is required.");

        RuleFor(x => x.ProviderId)
            .NotEmpty().WithMessage("ProviderId is required.");

        RuleFor(x => x.OrderItem)
            .NotNull().WithMessage("OrderItems are required.")
            .NotEmpty().WithMessage("Order must contain at least one item.");

        RuleFor(x => x.OrderItem)
            .SetValidator(new OrderItemRequestValidator());

    }
}