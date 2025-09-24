using Ecommerce.Entities.DTO.ServiceCatalog;
using FluentValidation;

namespace Ecommerce.API.Validators.ServiceCatalog
{
    public class CreateServiceRequestValidator : AbstractValidator<CreateServiceRequest>
    {
        private readonly string[] _allowedExtensions = { ".png", ".jpg", ".jpeg", ".jfif" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB in bytes

        public CreateServiceRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(150).WithMessage("Title must not exceed 150 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(x => x.Image)
                .NotNull().WithMessage("Image is required.")
                .Must(file =>
                {
                    if (file == null) return false;

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    return _allowedExtensions.Contains(ext);
                })
                .WithMessage($"Image must be one of the following types: {string.Join(", ", _allowedExtensions)}")
                .Must(file => file.Length <= _maxFileSize)
                .WithMessage("Image size must be less than or equal to 5 MB.");
        }
    }
}
