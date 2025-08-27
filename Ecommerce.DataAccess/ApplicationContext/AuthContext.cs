using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Models.Auth.UserTokens;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.DataAccess.ApplicationContext
{
    public class AuthContext : IdentityDbContext<User, Role, string>, IDataProtectionKeyContext
    {
        public AuthContext(DbContextOptions<AuthContext> options)
             : base(options)
        {
        }

        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}
