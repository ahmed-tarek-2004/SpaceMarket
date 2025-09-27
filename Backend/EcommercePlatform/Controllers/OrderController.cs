using Ecommerce.DataAccess.Services.Order;
using Ecommerce.Entities.DTO.Order;
using Ecommerce.Entities.DTO.Shared;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrderController(IOrderService orderService, ResponseHandler responseHandler, IValidator<RequestFilters<OrderSorting>> orderSortingValidator,IValidator<RequestServiceDto>serviceRequestValidator) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;
    private readonly ResponseHandler _responseHandler = responseHandler;
    private readonly IValidator<RequestFilters<OrderSorting>> _orderSortingValidator = orderSortingValidator;
    private readonly IValidator<RequestServiceDto> _serviceRequestValidator = serviceRequestValidator;

    [HttpGet("client")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetOrders([FromQuery] OrderFilters<OrderSorting> filters, CancellationToken cancellationToken)
    {

        var validationResult = await _orderSortingValidator.ValidateAsync(filters);
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                _responseHandler.BadRequest<object>(errors));
        }

        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _orderService.GetAllOrdersAsync(clientId!, filters, cancellationToken);

        return StatusCode((int)orders.StatusCode, orders);
    }

    [HttpGet("{orderId:guid}/client")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetClientOrders([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));
        }

        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _orderService.GetOrderByIdAsync(clientId!, orderId, cancellationToken);

        return StatusCode((int)orders.StatusCode, orders);
    }
    [HttpPost("client/request-service")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<Response<OrderResponse>>> RequestServiceAsync([FromQuery]Guid Serviceid, [FromQuery]Guid DataSetId,[FromForm] RequestServiceDto request)
    {

        var validationResult = await _serviceRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                _responseHandler.BadRequest<object>(errors));
        }

        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _orderService.RequestServiceAsync(clientId!,Serviceid,DataSetId, request);
        return StatusCode((int)result.StatusCode, result);
    }
}
