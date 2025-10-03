using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Reviews
{
    public class ProviderReplyRequest
    {
        public string ReviewId { get; set; }
        public string ReplyText { get; set; } = default!;
    }
}
