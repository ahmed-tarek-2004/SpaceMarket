using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.DTO.DebrisTracking;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.DebrisAlert
{
    public interface IDebrisAlertService
    {
        Task<Response<string>> RunDebrisCheckAsync(DateTime epochUtc);
        Task<Response<Guid>> RegisterSatelliteAsync(RegisterSatelliteRequest request, string userId);
        Task<Response<string>> SetThresholdAsync(Guid satelliteId, UpdateThresholdRequest request, string userId);
        Task<Response<List<CollisionAlertResponse>>> GetAlertHistoryAsync(string userId);
        Task<Response<List<string>>> GetAllCatalogSatelliteNamesAsync();
        Task<Response<PositionDto>> GetSatellitePositionAsync(Guid satelliteId);
    }
}
