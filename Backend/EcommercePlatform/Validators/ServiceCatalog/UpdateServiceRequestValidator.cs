using Ecommerce.Entities.DTO.ServiceCatalog;
using FluentValidation;

namespace Ecommerce.API.Validators.ServiceCatalog
{
    public class UpdateServiceRequestValidator : AbstractValidator<UpdateServiceRequest>
    {
        private readonly string[] _allowedExtensions = { ".png", ".jpg", ".jpeg", ".jfif" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB

        public UpdateServiceRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Service Id is required.");

            When(x => !string.IsNullOrEmpty(x.Title), () =>
            {
                RuleFor(x => x.Title)
                    .MaximumLength(150).WithMessage("Title must not exceed 150 characters.");
            });

            When(x => !string.IsNullOrEmpty(x.Description), () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
            });

            When(x => x.Price.HasValue, () =>
            {
                RuleFor(x => x.Price!.Value)
                    .GreaterThan(0).WithMessage("Price must be greater than zero.");
            });

            When(x => x.CategoryId.HasValue, () =>
            {
                RuleFor(x => x.CategoryId!.Value)
                    .NotEmpty().WithMessage("Category is required.");
            });

            // Image is optional but if present validate extension and size
            When(x => x.Image != null, () =>
            {
                RuleFor(x => x.Image)
                    .Must(file =>
                    {
                        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                        return _allowedExtensions.Contains(ext);
                    })
                    .WithMessage($"Image must be one of the following types: {string.Join(", ", _allowedExtensions)}")
                    .Must(file => file.Length <= _maxFileSize)
                    .WithMessage("Image size must be less than or equal to 5 MB.");
            });
        }
    }
}
