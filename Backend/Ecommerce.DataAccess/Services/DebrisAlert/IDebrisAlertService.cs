using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.DTO.DebrisTracking;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.DebrisAlert
{
    public interface IDebrisAlertService
    {
        Task<Response<string>> RunDebrisCheckAsync(DateTime epochUtc);
        Task<Response<Guid>> RegisterSatelliteAsync(RegisterSatelliteRequest request, string userId);
        Task<Response<string>> SetThresholdAsync(Guid satelliteId, UpdateThresholdRequest request, string userId);
        Task<Response<List<CollisionAlertResponse>>> GetAlertHistoryAsync(string userId);
<<<<<<< HEAD
        Task<Response<List<SatelliteResponseDto>>> GetMySatellitesAsync(string userId);
        //Task<Response<List<SatelliteCatalogResponseDto>>>GetSatelliteCatalogs(SatelliteCatalogFilter filter);
=======
        Task<Response<List<string>>> GetAllCatalogSatelliteNamesAsync();
        Task<Response<PositionDto>> GetSatellitePositionAsync(Guid satelliteId);
>>>>>>> feature/debris/debris-alert-tracking
    }
}
