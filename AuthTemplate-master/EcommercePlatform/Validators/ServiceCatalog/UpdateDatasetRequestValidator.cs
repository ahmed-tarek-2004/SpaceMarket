using Ecommerce.Entities.DTO.ServiceCatalog;
using FluentValidation;

namespace Ecommerce.API.Validators.ServiceCatalog
{
    public class UpdateDatasetRequestValidator : AbstractValidator<UpdateDatasetRequest>
    {
        public UpdateDatasetRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Dataset Id is required.");
        }
    }
}
