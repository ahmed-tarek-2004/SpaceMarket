using Ecommerce.Entities.DTO.Payment;
using FluentValidation;

namespace Ecommerce.API.Validators.Payment
{
    public class HandlePamentValidator : AbstractValidator<HandlePayment>
    {
        public HandlePamentValidator()
        {
            RuleFor(x => x.SessionId)
               .NotEmpty()
               .WithMessage("Session Id is required");
            RuleFor(x => x.OrderId)
               .NotEmpty()
               .WithMessage("OrderId must not be empty.");
        }
    }
}
