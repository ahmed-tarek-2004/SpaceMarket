using Azure.Core;
using Ecommerce.DataAccess.Services.ServiceCatalog;
using Ecommerce.Entities.DTO.ServiceCatalog;
using Ecommerce.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class ServiceController : ControllerBase
{
    private readonly IServiceCatalogService _serviceCatalogService;
    private readonly ResponseHandler _responseHandler;
    private readonly IValidator<CreateServiceRequest> _createValidator;
    private readonly IValidator<UpdateServiceRequest> _updateValidator;
    private readonly IValidator<ServiceMetricsFilterRequest> _metricsValidator;
    private readonly IValidator<UpdateServiceStatusRequest> _statusValidator;
    private readonly IValidator<ServiceListFilterRequest> _listFilterValidator;
    private readonly IValidator<ServiceFilterRequest> _filterValidator;


    public ServiceController(IServiceCatalogService serviceCatalogService,
                             ResponseHandler responseHandler,
                             IValidator<CreateServiceRequest> createValidator,
                             IValidator<UpdateServiceRequest> updateValidator,
                             IValidator<ServiceMetricsFilterRequest> metricsValidator,
                             IValidator<UpdateServiceStatusRequest> statusValidator,
                             IValidator<ServiceListFilterRequest> listFilterValidator,
                             IValidator<ServiceFilterRequest> filterValidator)
    {
        _serviceCatalogService = serviceCatalogService;
        _responseHandler = responseHandler;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _metricsValidator = metricsValidator;
        _statusValidator = statusValidator;
        _listFilterValidator = listFilterValidator;
        _filterValidator = filterValidator;
    }

    [HttpPost("create")]
    [Authorize(Roles = "ServiceProvider")]
    public async Task<ActionResult<Response<ServiceResponse>>> Create([FromForm] CreateServiceRequest request)
    {
        ValidationResult validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                _responseHandler.BadRequest<object>(errors));
        }

        var providerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var response = await _serviceCatalogService.CreateServiceAsync(providerId!, request);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut("update")]
    [Authorize(Roles = "ServiceProvider")]
    public async Task<ActionResult<Response<ServiceResponse>>> Update([FromForm] UpdateServiceRequest request)
    {
        ValidationResult validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                _responseHandler.BadRequest<object>(errors));
        }

        var providerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var response = await _serviceCatalogService.UpdateServiceAsync(providerId!, request);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ServiceProvider")]

    public async Task<ActionResult<Response<bool>>> Delete(Guid id)
    {
        var providerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var response = await _serviceCatalogService.DeleteServiceAsync(providerId!, id);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("my-services")]
    [Authorize(Roles = "ServiceProvider")]
    public async Task<ActionResult<Response<List<ServiceResponse>>>> GetMyServices()
    {
        var providerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var response = await _serviceCatalogService.GetMyServicesAsync(providerId!);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("metrics")]
    [Authorize(Roles = "ServiceProvider")]
    public async Task<ActionResult<Response<List<ServiceMetricsResponse>>>> GetServiceMetrics([FromQuery] ServiceMetricsFilterRequest filter)
    {
        var providerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(providerId))
            return Unauthorized(_responseHandler.Unauthorized<List<ServiceMetricsResponse>>("Unauthorized"));

        var validation = await _metricsValidator.ValidateAsync(filter);
        if (!validation.IsValid)
        {
            string errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
            return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                _responseHandler.BadRequest<object>(errors));
        }

        var result = await _serviceCatalogService.GetServiceMetricsAsync(providerId, filter);
        return StatusCode((int)result.StatusCode, result);
    }

    // ---------------- Admin Endpoints ----------------

    /// <summary>
    /// Admin: list all services with filtering (category, provider, status, paging)
    /// </summary>
    [HttpGet("admin/list")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response<System.Collections.Generic.List<ServiceResponse>>>> GetAllServices([FromQuery] ServiceListFilterRequest filter)
    {
        var validation = await _listFilterValidator.ValidateAsync(filter);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                              _responseHandler.BadRequest<object>(errors));
        }

        var response = await _serviceCatalogService.GetAllServicesAsync(filter);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Admin: update service status (Approve / Activate / Suspend / Delete(soft))
    /// </summary>
    [HttpPut("admin/update-status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response<bool>>> UpdateServiceStatus([FromBody] UpdateServiceStatusRequest request)
    {
        ValidationResult validationResult = await _statusValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                              _responseHandler.BadRequest<object>(errors));
        }

        var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(adminId))
            return Unauthorized(_responseHandler.Unauthorized<bool>("Unauthorized"));

        var response = await _serviceCatalogService.UpdateServiceStatusAsync(request, adminId);
        return StatusCode((int)response.StatusCode, response);
    }
    /// <summary>
    /// Ckient End Point For Retreving The Service
    /// </summary>
    
    [HttpGet("client/available-service")]
    [Authorize(Roles ="Client")]
    public async Task<ActionResult<List<ServiceFilterResponse>>> GetAvailableServicesAsync([FromQuery]ServiceFilterRequest filter)
    {
        ValidationResult validationResult = await _filterValidator.ValidateAsync(filter);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                              _responseHandler.BadRequest<object>(errors));
        }
        var response = await _serviceCatalogService.GetAvailableServicesAsync(filter);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("client/service-detail/{serviceId}")]
    [Authorize(Roles ="Client")]
    public async Task<ActionResult<ServiceResponse>> GetDetailedServiceAsync([FromRoute]Guid serviceId)
    {
       if(string.IsNullOrEmpty(serviceId.ToString()))
        {
            return StatusCode((int)_responseHandler.BadRequest<object>().StatusCode,
                             _responseHandler.BadRequest<object>("Service Must Not Be Empty"));
        }
        var response = await _serviceCatalogService.GetServiceDetailAsync(serviceId);
        return StatusCode((int)response.StatusCode, response);
    }

}
