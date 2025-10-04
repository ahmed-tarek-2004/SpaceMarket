using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.DebrisTracking
{
    public class SatelliteCatalogResponseDto
    {
        public Guid Id { get; set; }
        public string NoradId { get; set; }      
        public string Name { get; set; }        
        public string TleLine1 { get; set; }
        public string TleLine2 { get; set; }

    }
}
