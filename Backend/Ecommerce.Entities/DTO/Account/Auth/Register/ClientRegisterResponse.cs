using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Account.Auth.Register
{
    public class ClientRegisterResponse
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public bool isEmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }

    }
}
