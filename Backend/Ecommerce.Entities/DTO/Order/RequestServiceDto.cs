using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Order
{
    public class RequestServiceDto
    {
        //public Guid ServiceId { get; set; }
        //public string Scope { get; set; } = string.Empty; 
        public decimal Budget { get; set; }
        public int Quantity {  get; set; }
        public string ApiKey {  get; set; }
        public string DownloadUrl { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
    }
}
