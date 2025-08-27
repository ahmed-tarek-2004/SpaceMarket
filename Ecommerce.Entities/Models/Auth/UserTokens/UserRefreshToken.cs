using System.ComponentModel.DataAnnotations;

using Ecommerce.Entities.Models.Auth.Identity;

namespace Ecommerce.Entities.Models.Auth.UserTokens
{
    public class UserRefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string? Token { get; set; }
        public bool IsUsed { get; set; } 
        public DateTime ExpiryDateUtc { get; set; }
        public virtual User? User { get; set; }
    }
}
