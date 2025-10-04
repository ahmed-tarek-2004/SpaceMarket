using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Reviews
{
    public class UpdateReviewRequest
    {
        public string Id { get; set; }        
        public int? Rating { get; set; }        
        public string? Text { get; set; }       
    }
}
