using Ecommerce.Entities.DTO.DebrisTracking;
using Ecommerce.Entities.Shared;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.DebrisAlert
{
    public interface IDebrisAlertService
    {
        Task<Response<string>> RunDebrisCheckAsync(DateTime epochUtc);
        Task<Response<Guid>> RegisterSatelliteAsync(RegisterSatelliteRequest request, string userId);
        Task<Response<string>> SetThresholdAsync(Guid satelliteId, UpdateThresholdRequest request, string userId);
        Task<Response<List<CollisionAlertResponse>>> GetAlertHistoryAsync(string userId);

        Task<Response<List<SatelliteResponseDto>>> GetMySatellitesAsync(string userId);
        public Task<Response<PaginatedList<SatelliteCatalogResponseDto>>> GetSatelliteCatalogsAsync(SatelliteCatalogFilter filter);

        Task<Response<List<string>>> GetAllCatalogSatelliteNamesAsync();
        Task<Response<PositionDto>> GetSatellitePositionAsync(Guid satelliteId);

    }
}
