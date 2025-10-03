using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.DebrisTracking
{
    public class SatelliteCatalogFilter
    {
        public string? Name {  get; set; }
        public string? NoradId {  get; set; }

        public string? TleLine1 { get; set; }
        public string? TleLine2 { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
