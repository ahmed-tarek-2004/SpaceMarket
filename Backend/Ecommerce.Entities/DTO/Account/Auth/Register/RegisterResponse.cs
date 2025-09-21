namespace Ecommerce.Entities.DTO.Account.Auth.Register
{
    public class RegisterResponse
    {
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public string Role { get; set; }
        public string RefreshToken { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
