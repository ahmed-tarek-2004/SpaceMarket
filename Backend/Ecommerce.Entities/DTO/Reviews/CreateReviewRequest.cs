using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Reviews
{
    public class CreateReviewRequest
    {
        public string ServiceId { get; set; }
        public string ProviderId { get; set; }
        public int Rating { get; set; } // 1–5
        public string? Text { get; set; }
    }
}
