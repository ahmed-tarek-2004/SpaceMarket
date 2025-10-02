using Ecommerce.Entities.DTO.ServiceCatalog;
using FluentValidation;

namespace Ecommerce.API.Validators.ServiceCatalog
{
    public class CreateDatasetRequestValidator : AbstractValidator<CreateDatasetRequest>
    {
        private readonly string[] _allowedExtensions = { ".png", ".jpg", ".jpeg", ".jfif" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB in bytes
        public CreateDatasetRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Dataset title is required.")
                .MaximumLength(200).WithMessage("Title must be less than 200 characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be >= 0.");

            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category Id is required.");

            When(x => x.File == null && string.IsNullOrWhiteSpace(x.ApiEndpoint), () =>
            {
                RuleFor(x => x.File)
                    .NotNull().WithMessage("Either File or ApiEndpoint is required.");
            });

            RuleFor(x => x.Thumbnail)
            .Must(file =>
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                return _allowedExtensions.Contains(ext);
            })
            .When(x => x.Thumbnail != null)
            .WithMessage($"Thumbnail must be one of the following types: {string.Join(", ", _allowedExtensions)}");

            RuleFor(x => x.Thumbnail)
                        .Must(file => file.Length <= _maxFileSize)
                        .When(x => x.Thumbnail != null)
                        .WithMessage("Thumbnail size must be less than or equal to 5 MB.");

        }            
    }
}
