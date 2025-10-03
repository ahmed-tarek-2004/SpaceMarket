namespace Ecommerce.Entities.DTO.Account.Auth.Register
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Password { get; set; }
    }
}
