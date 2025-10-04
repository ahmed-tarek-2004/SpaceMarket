using Ecommerce.Entities.DTO.Order;
using FluentValidation;

namespace Ecommerce.API.Validators.ServiceCatalog
{
    public class RequestServiceDtoValidator : AbstractValidator<RequestServiceDto>
    {
        public RequestServiceDtoValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Service Quantity Must be more than 0");
            RuleFor(x => x.Budget)
                .GreaterThan(0)//Will decide it later with final MVP
                .WithMessage("Service Budget Must no be more than 0");
            RuleFor(x => x.ApiKey)
                .NotEmpty()
                .WithMessage("ApiKey Must be not empty");
            RuleFor(x => x.DownloadUrl)
                .NotEmpty()
                .WithMessage("DownloadUrl Must be not empty");
            //RuleFor(x => x.DataSetId)
            //    .NotNull()
            //    .WithMessage("DataSetId Must be not empty");

        }
    }
}
