using Ecommerce.Entities.DTO.ServiceCatalog;
using FluentValidation;

namespace Ecommerce.API.Validators.ServiceCatalog
{
    public class UpdateDatasetStatusRequestValidator : AbstractValidator<UpdateDatasetStatusRequest>
    {
        public UpdateDatasetStatusRequestValidator()
        {
            RuleFor(x => x.DatasetId)
                .NotEmpty().WithMessage("Dataset Id is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => new[] { "Active", "PendingApproval", "Suspended" }
                .Contains(s)).WithMessage("Invalid status value.");
        }
    }
}
