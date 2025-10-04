using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.DebrisAlert;
using Ecommerce.DataAccess.Services.Notifications;
using Ecommerce.DataAccess.Services.OrbitalPropagation;
using Ecommerce.Entities.DTO.DebrisTracking;
using Ecommerce.Entities.DTO.ServiceCatalog;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Ecommerce.DataAccess.Services.DebrisTracking
{
    public class DebrisAlertService : IDebrisAlertService
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrbitalPropagationService _propagationService;
        private readonly INotificationService _notificationService;
        private readonly ResponseHandler _responseHandler;
        private readonly ILogger<DebrisAlertService> _logger;

        public DebrisAlertService(
            ApplicationDbContext context,
            IOrbitalPropagationService propagationService,
            INotificationService notificationService,
            ResponseHandler responseHandler,
            ILogger<DebrisAlertService> logger)
        {
            _context = context;
            _propagationService = propagationService;
            _notificationService = notificationService;
            _responseHandler = responseHandler;
            _logger = logger;
        }

        /// <summary>
        /// Register a satellite for a client from catalog
        /// </summary>
        public async Task<Response<Guid>> RegisterSatelliteAsync(RegisterSatelliteRequest request, string userId)
        {
            var catalogSat = await _context.SatellitesCatalog.FindAsync(request.CatalogSatelliteId);
            if (catalogSat == null)
                return _responseHandler.NotFound<Guid>("Satellite not found in catalog");

            var existingSatellite = await _context.Satellites
                    .FirstOrDefaultAsync(s => s.NoradId == catalogSat.NoradId);

            if (existingSatellite != null && existingSatellite.ClientId != userId)
                return _responseHandler.BadRequest<Guid>("Satellite is registered by another user");

            var satellite = new Satellite
            {
                Id = Guid.NewGuid(),
                ClientId = userId,
                Name = request.Name ?? catalogSat.Name,
                NoradId = catalogSat.NoradId,
                TleLine1 = catalogSat.TleLine1,
                TleLine2 = catalogSat.TleLine2,
                ProximityThresholdKm = request.ProximityThresholdKm,
                CreatedAt = DateTime.UtcNow
            };

            _context.Satellites.Add(satellite);
            await _context.SaveChangesAsync();

            // notify
            await _notificationService.NotifyUserAsync(
                recipientId: userId,
                senderId: "System",
                title: "Satellite Registered",
                message: $"Satellite {satellite.Name} has been registered successfully.",
                payload: satellite
            );

            return _responseHandler.Success<Guid>(satellite.Id, "Satellite registered successfully");
        }

        /// <summary>
        /// Update proximity threshold for a satellite
        /// </summary>
        public async Task<Response<string>> SetThresholdAsync(Guid satelliteId, UpdateThresholdRequest request, string userId)
        {
            var satellite = await _context.Satellites
                .FirstOrDefaultAsync(s => s.Id == satelliteId && s.ClientId == userId);

            if (satellite == null)
                return _responseHandler.NotFound<string>("Satellite not found");

            satellite.ProximityThresholdKm = request.ProximityThresholdKm;
            satellite.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // notify
            await _notificationService.NotifyUserAsync(
                recipientId: userId,
                senderId: "System",
                title: "Threshold Updated",
                message: $"Proximity threshold for satellite {satellite.Name} updated to {request.ProximityThresholdKm} km.",
                payload: satellite
            );

            return _responseHandler.Success<string>(null, "Threshold updated successfully");
        }

        /// <summary>
        /// Get historical alerts for a client
        /// </summary>
        public async Task<Response<List<CollisionAlertResponse>>> GetAlertHistoryAsync(string userId)
        {
            var alerts = await _context.CollisionAlerts
                .Include(a => a.Satellite)
                .Include(a => a.Debris)
                .Where(a => a.Satellite.ClientId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Select(a => new CollisionAlertResponse
                {
                    SatelliteId = a.SatelliteId,
                    SatelliteName = a.Satellite.Name,
                    SatellitePosition = new PositionDto
                    {
                        Latitude = a.SatLatitude,
                        Longitude = a.SatLongitude,
                        AltitudeKm = a.SatAltitudeKm
                    },
                    DebrisId = a.DebrisId,
                    DebrisName = a.Debris.Name,
                    DebrisPosition = new PositionDto
                    {
                        Latitude = a.DebrisLatitude,
                        Longitude = a.DebrisLongitude,
                        AltitudeKm = a.DebrisAltitudeKm
                    },
                    ClosestDistanceKm = a.ClosestDistanceKm,
                    Timestamp = a.Timestamp,
                    Status = a.Status.ToString()
                })
                .ToListAsync();

            return _responseHandler.Success(alerts, "Alert history retrieved");
        }
        /// <summary>
        /// Get all satellites registered by a client
        /// </summary>
        public async Task<Response<List<SatelliteResponseDto>>> GetMySatellitesAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                return _responseHandler.Unauthorized<List<SatelliteResponseDto>>("user not found");

            var satellites = await _context.Satellites
                .Where(s => s.ClientId == userId)
                .Select(s => new SatelliteResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    NoradId = s.NoradId,
                    ProximityThresholdKm = s.ProximityThresholdKm,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            return _responseHandler.Success(satellites, "Satellites retrieved successfully");
        }

        /// <summary>
        /// Main job entry → run propagation & detect collisions
        /// </summary>
        public async Task<Response<string>> RunDebrisCheckAsync(DateTime epochUtc)
        {
            try
            {
                var satellites = await _context.Satellites.Include(s => s.Client).ToListAsync();
                if (!satellites.Any())
                    return _responseHandler.Success<string>(null, "No satellites registered.");

                var debrisList = await _context.Debris.ToListAsync();
                if (!debrisList.Any())
                    return _responseHandler.Success<string>(null, "No debris data available.");

                var alerts = new List<CollisionAlert>();

                foreach (var sat in satellites)
                {
                    var satPos = _propagationService.PropagateToGeodetic(
                        sat.TleLine1, sat.TleLine2, epochUtc);

                    foreach (var debris in debrisList)
                    {
                        var debrisPos = _propagationService.PropagateToGeodetic(
                            debris.TleLine1, debris.TleLine2, epochUtc);

                        var distance = ComputeDistanceKm(satPos, debrisPos);

                        if (distance <= sat.ProximityThresholdKm)
                        {
                            var alert = new CollisionAlert
                            {
                                Id = Guid.NewGuid(),
                                SatelliteId = sat.Id,
                                DebrisId = debris.Id,
                                ClosestDistanceKm = distance,
                                Timestamp = epochUtc,
                                Status = Utilities.Enums.CollisionAlertStatus.New
                            };

                            alerts.Add(alert);

                            var alertResponse = new CollisionAlertResponse
                            {
                                SatelliteId = sat.Id,
                                SatelliteName = sat.Name,
                                SatellitePosition = satPos,
                                DebrisId = debris.Id,
                                DebrisName = debris.Name,
                                DebrisPosition = debrisPos,
                                ClosestDistanceKm = distance,
                                Timestamp = epochUtc,
                                Status = "New"
                            };

                            await _notificationService.NotifyUserAsync(
                                recipientId: sat.ClientId,
                                senderId: "System",
                                title: $"Collision Alert for {sat.Name}",
                                message: $"Debris {debris.NoradId} is within {distance:F2} km of satellite {sat.Name}.",
                                payload: alertResponse
                            );
                        }
                    }
                }

                if (alerts.Any())
                {
                    _context.CollisionAlerts.AddRange(alerts);
                    await _context.SaveChangesAsync();
                }

                return _responseHandler.Success<string>(null, $"Debris check completed. Alerts generated: {alerts.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RunDebrisCheckAsync");
                return _responseHandler.ServerError<string>("Error while checking debris alerts.");
            }
        }
        /// <summary>
        /// Get all satellite names from the catalog
        /// </summary>
        public async Task<Response<List<string>>> GetAllCatalogSatelliteNamesAsync()
        {
            var names = await _context.SatellitesCatalog
                .Select(s => s.Name)
                .ToListAsync();

            return _responseHandler.Success(names, "Satellite names retrieved successfully");
        }

        /// <summary>
        /// Get satellite position by Satellite Id
        /// </summary>
        public async Task<Response<PositionDto>> GetSatellitePositionAsync(Guid satelliteId)
        {
            var satellite = await _context.Satellites
                .FirstOrDefaultAsync(s => s.Id == satelliteId);

            if (satellite == null)
                return _responseHandler.NotFound<PositionDto>("Satellite not found");

            var position = _propagationService.PropagateToGeodetic(
                satellite.TleLine1, satellite.TleLine2, DateTime.UtcNow);

            return _responseHandler.Success(position, $"Position for satellite {satellite.Name} retrieved");
        }

        private double ComputeDistanceKm(PositionDto a, PositionDto b)
        {
            double R = 6371;

            var satX = (R + a.AltitudeKm) * Math.Cos(Deg2Rad(a.Latitude)) * Math.Cos(Deg2Rad(a.Longitude));
            var satY = (R + a.AltitudeKm) * Math.Cos(Deg2Rad(a.Latitude)) * Math.Sin(Deg2Rad(a.Longitude));
            var satZ = (R + a.AltitudeKm) * Math.Sin(Deg2Rad(a.Latitude));

            var debrisX = (R + b.AltitudeKm) * Math.Cos(Deg2Rad(b.Latitude)) * Math.Cos(Deg2Rad(b.Longitude));
            var debrisY = (R + b.AltitudeKm) * Math.Cos(Deg2Rad(b.Latitude)) * Math.Sin(Deg2Rad(b.Longitude));
            var debrisZ = (R + b.AltitudeKm) * Math.Sin(Deg2Rad(b.Latitude));

            var dx = satX - debrisX;
            var dy = satY - debrisY;
            var dz = satZ - debrisZ;

            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        private double Deg2Rad(double deg) => deg * Math.PI / 180.0;

        public async Task<Response<PaginatedList<SatelliteCatalogResponseDto>>> GetSatelliteCatalogsAsync(SatelliteCatalogFilter filter)
        {
            try
            {
                var SatelliteCatalog = _context.SatellitesCatalog.AsQueryable();
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    SatelliteCatalog = SatelliteCatalog.Where(q => q.Name == filter.Name);
                }
                if (!string.IsNullOrEmpty(filter.NoradId))
                {
                    SatelliteCatalog = SatelliteCatalog.Where(q => q.NoradId == filter.NoradId);
                }
                if (!string.IsNullOrEmpty(filter.TleLine1))
                {
                    SatelliteCatalog = SatelliteCatalog.Where(q => q.TleLine1 == filter.TleLine1);
                }
                if (!string.IsNullOrEmpty(filter.TleLine2))
                {
                    SatelliteCatalog = SatelliteCatalog.Where(q => q.TleLine2 == filter.TleLine2);
                }

                var response = SatelliteCatalog
                    .OrderBy(s => s.Id) 
                    .Select(s => new SatelliteCatalogResponseDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        NoradId = s.NoradId,
                        TleLine1 = s.TleLine1,
                        TleLine2 = s.TleLine2
                    });

               var paginated=  await PaginatedList<SatelliteCatalogResponseDto>.CreateAsync(response, filter.PageNumber, filter.PageSize);
                return _responseHandler.Success <PaginatedList<SatelliteCatalogResponseDto>> (paginated,"SatelieCatalog Retrieved Successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RunDebrisCheckAsync");
                return _responseHandler.ServerError<PaginatedList<SatelliteCatalogResponseDto>>("Error while checking debris alerts.");
            }
        }
    }
}
