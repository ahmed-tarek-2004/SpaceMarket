namespace Ecommerce.Entities.DTO.Account.Auth.Register;
public class RegisterServiceProviderResponse
{
    public string Id { get; set; }
    public string CompanyName { get; set; }
    public string WebsiteUrl { get; set; }
    public List<string> CertificationsUrls { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Status { get; set; }
    public string Role { get; set; }
}
