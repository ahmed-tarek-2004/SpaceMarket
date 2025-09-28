using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Reviews
{
    public class ReviewResponseDto
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public string ClientId { get; set; } = default!;
        public string ClientName { get; set; } = default!;
        public int Rating { get; set; }
        public string? Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ProviderReply { get; set; }
    }
}
