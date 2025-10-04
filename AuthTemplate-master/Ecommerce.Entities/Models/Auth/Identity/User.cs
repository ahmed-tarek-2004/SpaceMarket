using Ecommerce.Entities.Models.Auth.Users;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Entities.Models.Auth.Identity
{
    public class User : IdentityUser
    {
        public Client Client { get; set; }
        public Models.Auth.Users.ServiceProvider ServiceProvider { get; set; }
    }

}
