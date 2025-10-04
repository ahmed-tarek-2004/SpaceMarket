using Ecommerce.Entities.DTO.Payment;
using FluentValidation;

namespace Ecommerce.API.Validators.Payment
{
    public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
    {
        public PaymentRequestValidator()
        {
            RuleFor(x => x.ServiceName)
                .NotEmpty().WithMessage("Service name is required")
                .MaximumLength(100).WithMessage("Service name must not exceed 100 characters");

            RuleFor(x => x.ServiceUnitAmount)
                .GreaterThan(0).WithMessage("Service unit amount must be greater than zero");

            //RuleFor(x => x.Quantity)
            //    .GreaterThan(0).WithMessage("Quantity must be at least 1");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .Length(3).WithMessage("Currency must be a 3-letter code")
                .Matches("^[a-zA-Z]{3}$").WithMessage("Currency must be alphabetic (e.g., USD)");

            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("OrderId is required");

            RuleFor(x => x.SuccessUrl)
                .NotEmpty().WithMessage("SuccessUrl is required");

            RuleFor(x => x.CancelUrl)
                .NotEmpty().WithMessage("CancelUrl is required");
        }
    }
}
