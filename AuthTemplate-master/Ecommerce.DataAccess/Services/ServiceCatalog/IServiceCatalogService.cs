using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.DTO.ServiceCatalog;
using Ecommerce.Entities.Shared;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.ServiceCatalog
{
    public interface IServiceCatalogService
    {
        Task<Response<ServiceResponse>> CreateServiceAsync(string providerId, CreateServiceRequest request);
        Task<Response<ServiceResponse>> UpdateServiceAsync(string providerId, UpdateServiceRequest request);
        Task<Response<bool>> DeleteServiceAsync(string providerId, Guid serviceId);
        Task<Response<List<ServiceResponse>>> GetMyServicesAsync(string providerId);
        Task<Response<List<ServiceMetricsResponse>>> GetServiceMetricsAsync(string providerId, ServiceMetricsFilterRequest filter);
        Task<Response<List<AdminServiceResponse>>> GetAllServicesAsync(ServiceListFilterRequest filter);
        Task<Response<bool>> UpdateServiceStatusAsync(UpdateServiceStatusRequest request, string adminId);
        public Task<Response<PaginatedList<ServiceFilterResponse>>> GetAvailableServicesAsync(ServiceFilterRequest filter);
        public  Task<Response<ServiceResponse>> GetServiceDetailAsync(Guid serviceId);


        Task<Response<DatasetResponse>> CreateDatasetAsync(string providerId, CreateDatasetRequest request);
        Task<Response<DatasetResponse>> UpdateDatasetAsync(string providerId, UpdateDatasetRequest request);
        Task<Response<bool>> DeleteDatasetAsync(string providerId, Guid datasetId);
        Task<Response<List<DatasetResponse>>> GetMyDatasetsAsync(string providerId);
        Task<Response<List<AdminDatasetResponse>>> GetAllDatasetsAsync(DatasetFilterRequest filter);
        Task<Response<bool>> UpdateDatasetStatusAsync(UpdateDatasetStatusRequest request, string adminId);
        Task<Response<PaginatedList<DatasetFilterResponse>>> GetAvailableDatasetsAsync(DatasetFilterRequest filter);
        Task<Response<DatasetResponse>> GetDatasetDetailAsync(Guid datasetId);

    }
}
