using Microsoft.AspNetCore.Http;

namespace Ecommerce.Entities.DTO.Account.Auth.Register;
public record RegisterServiceProviderRequest(
    string Email,
    string PhoneNumber,
    string Password,
    string CompanyName,
    string? WebsiteUrl,

    List<IFormFile>? CertificationFiles
);
