using Ecommerce.Entities.DTO.ServiceCategory;
using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class UpdateServiceCategoryRequestValidator : AbstractValidator<UpdateServiceCategoryRequest>
    {
        public UpdateServiceCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(300);
        }
    }

}
