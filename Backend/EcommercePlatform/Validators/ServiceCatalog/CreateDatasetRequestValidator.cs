using Ecommerce.Entities.DTO.ServiceCatalog;
using FluentValidation;

namespace Ecommerce.API.Validators.ServiceCatalog
{
    public class CreateDatasetRequestValidator : AbstractValidator<CreateDatasetRequest>
    {
        private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png"];
        private const long _maxFileSize = 5 * 1024 * 1024; 

        public CreateDatasetRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotNull().WithMessage("Dataset title is required.")
                .NotEmpty().WithMessage("Dataset title is required.")
                .MaximumLength(200).WithMessage("Title must be less than 200 characters.");

            RuleFor(x => x.Price)
                .NotNull().WithMessage("Price is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Price must be >= 0.");

            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage("Category Id is required.")
                .NotEmpty().WithMessage("Category Id is required.");

            RuleFor(x => x.ApiEndpoint)
               .NotEmpty().WithMessage("ApiEndpoint is required.");

            RuleFor(x => x)
                .Must(x => !(x.File == null && string.IsNullOrWhiteSpace(x.ApiEndpoint)))
                .WithMessage("Either File and  ApiEndpoint is required.");

            RuleFor(x => x.Thumbnail)
                .NotNull().WithMessage("Thumbnail is required.")
                .Must(file => file != null && _allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
                    .WithMessage($"Thumbnail must be one of the following types: {string.Join(", ", _allowedExtensions)}")
                .Must(file => file != null && file.Length <= _maxFileSize)
                    .WithMessage("Thumbnail size must be less than or equal to 5 MB.");
        }
    }
}
