using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.Email;
using Ecommerce.DataAccess.Services.ImageUploading;
using Ecommerce.Entities.DTO.Order;
using Ecommerce.Entities.DTO.ServiceCatalog;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Ecommerce.DataAccess.Services.ServiceCatalog
{
    public class ServiceCatalogService : IServiceCatalogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _imageUploadService;
        private readonly ResponseHandler _responseHandler;
        private readonly ILogger<ServiceCatalogService> _logger;
        private readonly IEmailService _emailService;

        public ServiceCatalogService(
            ApplicationDbContext context,
            IImageUploadService imageUploadService,
            ResponseHandler responseHandler,
            ILogger<ServiceCatalogService> logger,
            IEmailService emailService)
        {
            _context = context;
            _imageUploadService = imageUploadService;
            _responseHandler = responseHandler;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<Response<ServiceResponse>> CreateServiceAsync(string providerId, CreateServiceRequest request)
        {
            try
            {
                string? uploadedUrl = null;
                if (request.Image != null)
                {
                    uploadedUrl = await _imageUploadService.UploadAsync(request.Image);
                }

                var service = new Service
                {
                    Id = Guid.NewGuid(),
                    ProviderId = providerId,
                    Title = request.Title,
                    Description = request.Description,
                    CategoryId = request.CategoryId,
                    Price = request.Price,
                    ImagesUrl = uploadedUrl ?? string.Empty,
                    Status = ServiceStatus.PendingApproval,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Services.AddAsync(service);
                await _context.SaveChangesAsync();

                var category = await _context.ServiceCategories
                    .FirstOrDefaultAsync(c => c.Id == service.CategoryId);

                var responseDto = new ServiceResponse
                {
                    Id = service.Id,
                    ProviderId = service.ProviderId,
                    Title = service.Title,
                    Description = service.Description,
                    CategoryName = category?.Name,
                    Price = service.Price,
                    ImagesUrl = service.ImagesUrl,
                    Status = service.Status.ToString(),
                    CreatedAt = service.CreatedAt
                };

                return _responseHandler.Created(responseDto, "Service created successfully. Pending admin approval.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service for Provider: {ProviderId}", providerId);
                return _responseHandler.ServerError<ServiceResponse>("An error occurred while creating the service.");
            }
        }

        public async Task<Response<ServiceResponse>> UpdateServiceAsync(string providerId, UpdateServiceRequest request)
        {
            try
            {
                var service = await _context.Services
                    .Include(s => s.Category)
                    .FirstOrDefaultAsync(s => s.Id == request.Id && s.ProviderId == providerId && !s.IsDeleted);

                if (service == null)
                    return _responseHandler.NotFound<ServiceResponse>("Service not found.");

                if (!string.IsNullOrWhiteSpace(request.Title))
                    service.Title = request.Title;

                if (!string.IsNullOrWhiteSpace(request.Description))
                    service.Description = request.Description;

                if (request.CategoryId.HasValue)
                    service.CategoryId = request.CategoryId.Value;

                if (request.Price.HasValue)
                    service.Price = request.Price.Value;

                if (request.Image != null)
                {
                    var uploadedUrl = await _imageUploadService.UploadAsync(request.Image);
                    service.ImagesUrl = uploadedUrl;
                }


                service.UpdatedAt = DateTime.UtcNow;
                service.Status = ServiceStatus.PendingApproval;

                _context.Services.Update(service);
                await _context.SaveChangesAsync();

                // لو غيرت الكاتيجوري لازم تحمل الاسم تاني
                var category = await _context.ServiceCategories
                    .FirstOrDefaultAsync(c => c.Id == service.CategoryId);

                var responseDto = new ServiceResponse
                {
                    Id = service.Id,
                    ProviderId = service.ProviderId,
                    Title = service.Title,
                    Description = service.Description,
                    CategoryName = category?.Name,
                    Price = service.Price,
                    ImagesUrl = service.ImagesUrl,
                    Status = service.Status.ToString(),
                    CreatedAt = service.CreatedAt,
                    UpdatedAt = service.UpdatedAt
                };

                return _responseHandler.Success(responseDto, "Service updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service {ServiceId} for Provider: {ProviderId}", request.Id, providerId);
                return _responseHandler.ServerError<ServiceResponse>("An error occurred while updating the service.");
            }
        }

        public async Task<Response<bool>> DeleteServiceAsync(string providerId, Guid serviceId)
        {
            try
            {
                var service = await _context.Services
                    .FirstOrDefaultAsync(s => s.Id == serviceId && s.ProviderId == providerId && !s.IsDeleted);

                if (service == null)
                    return _responseHandler.NotFound<bool>("Service not found.");

                service.IsDeleted = true;
                service.DeletedAt = DateTime.UtcNow;

                _context.Services.Update(service);
                await _context.SaveChangesAsync();

                return _responseHandler.Success(true, "Service deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service {ServiceId} for Provider: {ProviderId}", serviceId, providerId);
                return _responseHandler.ServerError<bool>("An error occurred while deleting the service.");
            }
        }

        public async Task<Response<List<ServiceResponse>>> GetMyServicesAsync(string providerId)
        {
            try
            {
                var services = await _context.Services
                    .Where(s => s.ProviderId == providerId && !s.IsDeleted)
                    .Include(s => s.Category)
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(s => new ServiceResponse
                    {
                        Id = s.Id,
                        ProviderId = s.ProviderId,
                        Title = s.Title,
                        Description = s.Description,
                        CategoryName = s.Category.Name,
                        Price = s.Price,
                        ImagesUrl = s.ImagesUrl,
                        Status = s.Status.ToString(),
                        CreatedAt = s.CreatedAt,
                        UpdatedAt = s.UpdatedAt
                    })
                    .ToListAsync();

                return _responseHandler.Success(services, "Fetched provider services successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching services for Provider: {ProviderId}", providerId);
                return _responseHandler.ServerError<List<ServiceResponse>>("An error occurred while fetching your services.");
            }
        }

        public async Task<Response<List<ServiceMetricsResponse>>> GetServiceMetricsAsync(string providerId, ServiceMetricsFilterRequest filter)
        {
            try
            {
                _logger.LogInformation("Start fetching metrics for ProviderId: {ProviderId}", providerId);

                var start = filter.StartDate ?? DateTime.UtcNow.AddDays(-30);
                var end = filter.EndDate ?? DateTime.UtcNow;

                _logger.LogInformation("Metrics Date Range: Start={Start}, End={End}", start, end);

                var services = await _context.Services
                    .Where(s => s.ProviderId == providerId && !s.IsDeleted)
                    .Select(s => new { s.Id, s.Title })
                    .ToListAsync();

                if (!services.Any())
                {
                    _logger.LogWarning("No services found for ProviderId: {ProviderId}", providerId);
                    return _responseHandler.NotFound<List<ServiceMetricsResponse>>("No services found for this provider.");
                }

                var serviceIds = services.Select(s => s.Id).ToList();
                var events = await _context.ServiceMetrics
                    .Where(e => serviceIds.Contains(e.ServiceId)
                                && e.Timestamp >= start && e.Timestamp <= end)
                    .ToListAsync();

                _logger.LogInformation("Fetched {Count} metric events for ProviderId: {ProviderId}", events.Count, providerId);

                var metrics = services.Select(s => new ServiceMetricsResponse
                {
                    ServiceId = s.Id,
                    Title = s.Title,
                    ViewsCount = events.Count(e => e.ServiceId == s.Id && e.EventType == ServiceEventType.View),
                    ClicksCount = events.Count(e => e.ServiceId == s.Id && e.EventType == ServiceEventType.Click),
                    RequestsCount = events.Count(e => e.ServiceId == s.Id && e.EventType == ServiceEventType.Request)
                }).ToList();

                _logger.LogInformation("Metrics computed for {ServicesCount} services of ProviderId: {ProviderId}", metrics.Count, providerId);

                return _responseHandler.Success(metrics, "Metrics fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching service metrics for Provider: {ProviderId}", providerId);
                return _responseHandler.ServerError<List<ServiceMetricsResponse>>("An error occurred while fetching metrics.");
            }
        }

        public async Task<Response<List<AdminServiceResponse>>> GetAllServicesAsync(ServiceListFilterRequest filter)
        {
            try
            {
                var query = _context.Services
                    .Include(s => s.Category)
                    .Include(s => s.Provider)
                    .ThenInclude(sp => sp.User)
                    .AsQueryable();

                if (filter.CategoryId.HasValue)
                    query = query.Where(s => s.CategoryId == filter.CategoryId);

                if (!string.IsNullOrEmpty(filter.ProviderId))
                    query = query.Where(s => s.ProviderId == filter.ProviderId);

                if (!string.IsNullOrEmpty(filter.Status))
                    query = query.Where(s => s.Status.ToString() == filter.Status);

                var services = await query
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(s => new AdminServiceResponse
                    {
                        Id = s.Id,
                        Title = s.Title,
                        ProviderId = s.ProviderId,
                        ProviderName = s.Provider.CompanyName,
                        ProviderEmail = s.Provider.User.Email,
                        CategoryName = s.Category.Name,
                        Price = s.Price,
                        Status = s.Status.ToString(),
                        CreatedAt = s.CreatedAt
                    })
                    .ToListAsync();

                return _responseHandler.Success(services, "Services fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all services for admin");
                return _responseHandler.ServerError<List<AdminServiceResponse>>("An error occurred while fetching services.");
            }
        }

        public async Task<Response<bool>> UpdateServiceStatusAsync(UpdateServiceStatusRequest request, string adminId)
        {
            try
            {
                var service = await _context.Services
                    .Include(s => s.Provider)
                    .ThenInclude(sp => sp.User)
                    .FirstOrDefaultAsync(s => s.Id == request.ServiceId && !s.IsDeleted);

                if (service == null)
                    return _responseHandler.NotFound<bool>("Service not found.");

                // نحول الStatus من نص للـenum
                if (!Enum.TryParse<ServiceStatus>(request.Status, true, out var parsedStatus))
                {
                    return _responseHandler.BadRequest<bool>("Invalid status value.");
                }

                // Soft delete لو عايزين نحافظ على السجلات
                if (parsedStatus == ServiceStatus.Suspended && request.Status.Equals("Suspended", StringComparison.OrdinalIgnoreCase))
                {
                    service.Status = ServiceStatus.Suspended;
                }
                else
                {
                    service.Status = parsedStatus;
                }

                service.UpdatedAt = DateTime.UtcNow;

                _context.Services.Update(service);

                // AuditLog
                var audit = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = adminId,
                    Action = "ServiceStatusChanged",
                    EntityName = "Service",
                    EntityId = service.Id,
                    Details = $"Changed to {parsedStatus} Reason: {request.Reason ?? "N/A"}",
                    Timestamp = DateTime.UtcNow
                };
                await _context.AuditLogs.AddAsync(audit);

                await _context.SaveChangesAsync();

                // ممكن هنا تبعت Email أو Notification للProvider
                await _emailService.SendServiceStatusChangedEmailAsync(
                    service.Provider.User,
                    service.Title,
                    parsedStatus.ToString(),
                    request.Reason
                );

                return _responseHandler.Success(true, $"Service {parsedStatus} successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service status {ServiceId} by Admin {AdminId}", request.ServiceId, adminId);
                return _responseHandler.ServerError<bool>("An error occurred while updating the service status.");
            }
        }

        #region Client
        public async Task<Response<PaginatedList<ServiceFilterResponse>>> GetAvailableServicesAsync(ServiceFilterRequest filter)
        {
            try
            {
                var Services = _context.Services
                    .Include(s => s.Category)
                    .Include(s => s.Provider)
                    //.ThenInclude(s => s.User)
                    .Where(s => !s.IsDeleted && s.Status == ServiceStatus.Active)
                    .AsNoTracking();


                if (filter.CategoryId.HasValue)
                {
                    Services = Services.Where(s => s.CategoryId == filter.CategoryId);
                    _logger.LogInformation("Service Filterd By Category successfully ");
                }


                if (filter.MinPrice.HasValue)
                {
                    Services = Services.Where(s => s.Price >= filter.MinPrice.Value);
                    _logger.LogInformation("Service Filterd By MinPrice successfully");

                }

                if (filter.MaxPrice.HasValue)
                {
                    Services = Services.Where(s => s.Price <= filter.MaxPrice.Value);
                    _logger.LogInformation("Service Filterd By MaxPrice successfully");
                }
                //Note : Untill We Add It to Model Later
                //if (!string.IsNullOrEmpty(filter.Location))
                //   Services = Services.Where(s => s.Provider.User.Loaction.Contain(filter.Location));

                var services = Services
                    .AsNoTracking()
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(s => new ServiceFilterResponse
                    {
                        Id = s.Id,
                        CategoryId = s.CategoryId,
                        Title = s.Title,
                        Description = s.Description,
                        ProviderName = s.Provider.CompanyName,
                        CategoryName = s.Category.Name,
                        Price = s.Price
                    })
                    //.ToList();
                    //.AsNoTracking();
                    .AsQueryable();

                 var paginated = await PaginatedList<ServiceFilterResponse>.CreateAsync(services, filter.PageNumber, filter.PageSize);

                return _responseHandler.Success(paginated, "Services fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error When retrieving Service ");
                return _responseHandler.ServerError<PaginatedList<ServiceFilterResponse>>("An error occurred while retrieving the service.");
            }
        }

        public async Task<Response<ServiceResponse>> GetServiceDetailAsync(Guid serviceId)
        {
            try
            {
                var service = await _context.Services
                    .Include(s => s.Category)
                    .Include(s => s.Provider)
                    //.ThenInclude(p => p.User)
                    .AsNoTracking()
                   .FirstOrDefaultAsync(s => s.Id == serviceId && s.Status == ServiceStatus.Active && !s.IsDeleted);
                if (service == null)
                {
                    _logger.LogWarning("Service Not Found ");
                    return _responseHandler.BadRequest<ServiceResponse>($"Service With Id {serviceId} Not Found.");
                }

                var response = new ServiceResponse
                {
                    Id = serviceId,
                    Title = service.Title,
                    Description = service.Description,
                    ProviderId = service.Provider.Id,
                    CategoryName = service.Category.Name,
                    Price = service.Price,
                    ImagesUrl = service.ImagesUrl,
                    Status = service.Status.ToString(),
                    CreatedAt = service.CreatedAt,
                    UpdatedAt = service.UpdatedAt,
                };

                return _responseHandler.Success(response, $"Services {service.Id} fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Service ");
                return _responseHandler.ServerError<ServiceResponse>("An error occurred while creating the service.");
            }

        }
        #endregion

        // داخل ServiceCatalogService
        public async Task<Response<DatasetResponse>> CreateDatasetAsync(string providerId, CreateDatasetRequest request)
        {
            try
            {
                string? fileUrl = null;
                string? thumbnailUrl = null;

                if (request.File != null)
                    fileUrl = await _imageUploadService.UploadAsync(request.File);

                if (request.Thumbnail != null)
                    thumbnailUrl = await _imageUploadService.UploadAsync(request.Thumbnail);

                var dataset = new Dataset
                {
                    Id = Guid.NewGuid(),
                    ProviderId = providerId,
                    Title = request.Title,
                    Description = request.Description,
                    CategoryId = request.CategoryId,
                    Price = request.Price,
                    FileUrl = fileUrl,
                    ThumbnailUrl = thumbnailUrl,
                    ApiEndpoint = request.ApiEndpoint,
                    Status = ServiceStatus.PendingApproval,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Datasets.AddAsync(dataset);
                await _context.SaveChangesAsync();

                var responseDto = new DatasetResponse
                {
                    Id = dataset.Id,
                    Title = dataset.Title,
                    Description = dataset.Description,
                    CategoryId = dataset.CategoryId,
                    Price = dataset.Price,
                    FileUrl = dataset.FileUrl,
                    ThumbnailUrl = dataset.ThumbnailUrl,
                    ApiEndpoint = dataset.ApiEndpoint,
                    ProviderId = dataset.ProviderId,
                    ProviderName = dataset.Provider.CompanyName,
                    Status = dataset.Status.ToString(),
                    CreatedAt = dataset.CreatedAt
                };

                return _responseHandler.Created(responseDto, "Dataset created successfully. Pending admin approval.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dataset for Provider: {ProviderId}", providerId);
                return _responseHandler.ServerError<DatasetResponse>("An error occurred while creating the dataset.");
            }
        }

        public async Task<Response<DatasetResponse>> UpdateDatasetAsync(string providerId, UpdateDatasetRequest request)
        {
            try
            {
                var dataset = await _context.Datasets
                    .Include(d => d.Category)
                    .FirstOrDefaultAsync(d => d.Id == request.Id && d.ProviderId == providerId && !d.IsDeleted);

                if (dataset == null)
                    return _responseHandler.NotFound<DatasetResponse>("Dataset not found.");

                if (!string.IsNullOrWhiteSpace(request.Title))
                    dataset.Title = request.Title;

                if (!string.IsNullOrWhiteSpace(request.Description))
                    dataset.Description = request.Description;

                if (request.CategoryId.HasValue)
                    dataset.CategoryId = request.CategoryId.Value;

                if (request.Price.HasValue)
                    dataset.Price = request.Price.Value;

                if (request.File != null)
                    dataset.FileUrl = await _imageUploadService.UploadAsync(request.File);

                if (request.Thumbnail != null)
                    dataset.ThumbnailUrl = await _imageUploadService.UploadAsync(request.Thumbnail);

                dataset.UpdatedAt = DateTime.UtcNow;
                dataset.Status = ServiceStatus.PendingApproval;

                _context.Datasets.Update(dataset);
                await _context.SaveChangesAsync();

                var response = new DatasetResponse
                {
                    Id = dataset.Id,
                    Title = dataset.Title,
                    Description = dataset.Description,
                    CategoryId = dataset.CategoryId,
                    Price = dataset.Price,
                    FileUrl = dataset.FileUrl,
                    ThumbnailUrl = dataset.ThumbnailUrl,
                    ApiEndpoint = dataset.ApiEndpoint,
                    ProviderId = dataset.ProviderId,
                    ProviderName = dataset.Provider.CompanyName,
                    Status = dataset.Status.ToString(),
                    CreatedAt = dataset.CreatedAt,
                    UpdatedAt = dataset.UpdatedAt
                };

                return _responseHandler.Success(response, "Dataset updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating dataset {DatasetId} for Provider {ProviderId}", request.Id, providerId);
                return _responseHandler.ServerError<DatasetResponse>("An error occurred while updating the dataset.");
            }
        }

        public async Task<Response<bool>> DeleteDatasetAsync(string providerId, Guid datasetId)
        {
            try
            {
                var dataset = await _context.Datasets
                    .FirstOrDefaultAsync(d => d.Id == datasetId && d.ProviderId == providerId && !d.IsDeleted);

                if (dataset == null)
                    return _responseHandler.NotFound<bool>("Dataset not found.");

                dataset.IsDeleted = true;
                dataset.DeletedAt = DateTime.UtcNow;

                _context.Datasets.Update(dataset);
                await _context.SaveChangesAsync();

                return _responseHandler.Success(true, "Dataset deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting dataset {DatasetId} for Provider {ProviderId}", datasetId, providerId);
                return _responseHandler.ServerError<bool>("An error occurred while deleting the dataset.");
            }
        }

        public async Task<Response<List<DatasetResponse>>> GetMyDatasetsAsync(string providerId)
        {
            try
            {
                var datasets = await _context.Datasets
                    .Include(d => d.Category)
                    .Where(d => d.ProviderId == providerId && !d.IsDeleted)
                    .OrderByDescending(d => d.CreatedAt)
                    .Select(d => new DatasetResponse
                    {
                        Id = d.Id,
                        Title = d.Title,
                        Description = d.Description,
                        CategoryId = d.CategoryId,
                        Price = d.Price,
                        FileUrl = d.FileUrl,
                        ThumbnailUrl = d.ThumbnailUrl,
                        ApiEndpoint = d.ApiEndpoint,
                        ProviderId = d.ProviderId,
                        ProviderName = d.Provider.CompanyName,
                        Status = d.Status.ToString(),
                        CreatedAt = d.CreatedAt,
                        UpdatedAt = d.UpdatedAt
                    })
                    .ToListAsync();

                return _responseHandler.Success(datasets, "Fetched provider datasets successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching datasets for Provider {ProviderId}", providerId);
                return _responseHandler.ServerError<List<DatasetResponse>>("An error occurred while fetching your datasets.");
            }
        }

        public async Task<Response<List<AdminDatasetResponse>>> GetAllDatasetsAsync(DatasetFilterRequest filter)
        {
            try
            {
                var query = _context.Datasets
                    .Include(d => d.Category)
                    .Include(d => d.Provider)
                    .ThenInclude(p => p.User)
                    .AsQueryable();

                if (filter.CategoryId.HasValue)
                    query = query.Where(d => d.CategoryId == filter.CategoryId);

                if (!string.IsNullOrEmpty(filter.ProviderId))
                    query = query.Where(d => d.ProviderId == filter.ProviderId);

                if (!string.IsNullOrEmpty(filter.Status))
                    query = query.Where(d => d.Status.ToString() == filter.Status);

                var datasets = await query
                    .OrderByDescending(d => d.CreatedAt)
                    .Select(d => new AdminDatasetResponse
                    {
                        Id = d.Id,
                        Title = d.Title,
                        ProviderId = d.ProviderId,
                        ProviderName = d.Provider.CompanyName,
                        ProviderEmail = d.Provider.User.Email,
                        CategoryName = d.Category.Name,
                        Price = d.Price,
                        Status = d.Status.ToString(),
                        CreatedAt = d.CreatedAt
                    })
                    .ToListAsync();

                return _responseHandler.Success(datasets, "Datasets fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all datasets for admin");
                return _responseHandler.ServerError<List<AdminDatasetResponse>>("An error occurred while fetching datasets.");
            }
        }

        public async Task<Response<bool>> UpdateDatasetStatusAsync(UpdateDatasetStatusRequest request, string adminId)
        {
            try
            {
                var dataset = await _context.Datasets
                    .Include(d => d.Provider)
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(d => d.Id == request.DatasetId && !d.IsDeleted);

                if (dataset == null)
                    return _responseHandler.NotFound<bool>("Dataset not found.");

                if (!Enum.TryParse<ServiceStatus>(request.Status, true, out var parsedStatus))
                    return _responseHandler.BadRequest<bool>("Invalid status value.");

                dataset.Status = parsedStatus;
                dataset.UpdatedAt = DateTime.UtcNow;

                _context.Datasets.Update(dataset);

                await _context.SaveChangesAsync();

                await _emailService.SendServiceStatusChangedEmailAsync(
                    dataset.Provider.User,
                    dataset.Title,
                    parsedStatus.ToString(),
                    request.Reason
                );

                return _responseHandler.Success(true, $"Dataset {parsedStatus} successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating dataset status {DatasetId} by Admin {AdminId}", request.DatasetId, adminId);
                return _responseHandler.ServerError<bool>("An error occurred while updating the dataset status.");
            }
        }

        public async Task<Response<PaginatedList<DatasetFilterResponse>>> GetAvailableDatasetsAsync(DatasetFilterRequest filter)
        {
            try
            {
                var datasets = _context.Datasets
                    .Include(d => d.Category)
                    .Include(d => d.Provider)
                    .Where(d => !d.IsDeleted && d.Status == ServiceStatus.Active)
                    .AsNoTracking();

                if (filter.CategoryId.HasValue)
                    datasets = datasets.Where(d => d.CategoryId == filter.CategoryId);

                if (filter.MinPrice.HasValue)
                    datasets = datasets.Where(d => d.Price >= filter.MinPrice.Value);

                if (filter.MaxPrice.HasValue)
                    datasets = datasets.Where(d => d.Price <= filter.MaxPrice.Value);

                var query = datasets
                    .OrderByDescending(d => d.CreatedAt)
                    .Select(d => new DatasetFilterResponse
                    {
                        Id = d.Id,
                        CategoryId = d.CategoryId,
                        Title = d.Title,
                        Description = d.Description,
                        ProviderName = d.Provider.CompanyName,
                        Price = d.Price,
                        ThumbnailUrl = d.ThumbnailUrl
                    });

                var paginated = await PaginatedList<DatasetFilterResponse>.CreateAsync(query, filter.PageNumber, filter.PageSize);

                return _responseHandler.Success(paginated, "Datasets fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving datasets");
                return _responseHandler.ServerError<PaginatedList<DatasetFilterResponse>>("An error occurred while retrieving datasets.");
            }
        }

        public async Task<Response<DatasetResponse>> GetDatasetDetailAsync(Guid datasetId)
        {
            try
            {
                var dataset = await _context.Datasets
                    .Include(d => d.Category)
                    .Include(d => d.Provider)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == datasetId && d.Status == ServiceStatus.Active && !d.IsDeleted);

                if (dataset == null)
                    return _responseHandler.NotFound<DatasetResponse>("Dataset not found.");

                var response = new DatasetResponse
                {
                    Id = dataset.Id,
                    Title = dataset.Title,
                    Description = dataset.Description,
                    CategoryId = dataset.CategoryId,
                    Price = dataset.Price,
                    FileUrl = dataset.FileUrl,
                    ThumbnailUrl = dataset.ThumbnailUrl,
                    ApiEndpoint = dataset.ApiEndpoint,
                    ProviderId = dataset.ProviderId,
                    ProviderName = dataset.Provider.CompanyName,
                    Status = dataset.Status.ToString(),
                    CreatedAt = dataset.CreatedAt,
                    UpdatedAt = dataset.UpdatedAt
                };

                return _responseHandler.Success(response, "Dataset fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dataset detail {DatasetId}", datasetId);
                return _responseHandler.ServerError<DatasetResponse>("An error occurred while retrieving the dataset.");
            }
        }

    }
}
