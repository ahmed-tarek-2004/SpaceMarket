using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Account.Auth.Register
{
    public class ClientRegisterRequest
    {
        public string FullName { get; set; }
        public string OrganizationName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Country { get; set; }
    }
}
