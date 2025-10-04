using Ecommerce.DataAccess.Services.Reviews;
using Ecommerce.Entities.DTO.Reviews;
using Ecommerce.Entities.Shared.Bases;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;
        private readonly ResponseHandler _responseHandler;
        private readonly ILogger<ReviewsController> _logger;
        private readonly IValidator<CreateReviewRequest> _createValidator;
        private readonly IValidator<UpdateReviewRequest> _updateValidator;
        private readonly IValidator<ProviderReplyRequest> _replyValidator;

        public ReviewsController(IReviewService service,
                                 ResponseHandler responseHandler,
                                 ILogger<ReviewsController> logger,
                                 IValidator<CreateReviewRequest> createValidator,
                                 IValidator<UpdateReviewRequest> updateValidator,
                                 IValidator<ProviderReplyRequest> replyValidator)
        {
            _service = service;
            _responseHandler = responseHandler;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _replyValidator = replyValidator;
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation("Create review {@Request}", request);

            var validation = await _createValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                string errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                                  _responseHandler.BadRequest<object>(errors));
            }

            var result = await _service.CreateAsync(request,clientId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Update([FromBody] UpdateReviewRequest request)
        {
            _logger.LogInformation("Update review {@Request}", request);

            var validation = await _updateValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                string errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                                  _responseHandler.BadRequest<object>(errors));
            }

            var result = await _service.UpdateAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogWarning("Delete review {Id} for client {Client}", id, clientId);
            var result = await _service.DeleteAsync(id, clientId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("service/{serviceId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByService(Guid serviceId)
        {
            var result = await _service.GetByServiceAsync(serviceId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("provider")]
        [Authorize(Roles = "ServiceProvider,Admin")]
        public async Task<IActionResult> GetForProvider()
        {
            // get provider id from JWT
            var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _service.GetForProviderAsync(providerId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetFiltered([FromBody] ReviewFilterRequest filter)
        {
            var result = await _service.GetAllFilteredAsync(filter);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("reply")]
        [Authorize(Roles = "ServiceProvider,Admin")]
        public async Task<IActionResult> ProviderReply([FromBody] ProviderReplyRequest request)
        {
            var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation("Provider reply {@Request}", request);

            var validation = await _replyValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                string errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                                  _responseHandler.BadRequest<object>(errors));
            }

            var result = await _service.ProviderReplyAsync(request, providerId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("admin/hide-delete/{reviewId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminHideOrDelete(Guid reviewId, [FromQuery] bool delete)
        {
            _logger.LogWarning("Admin hide/delete review {Id}, delete={Delete}", reviewId, delete);
            var result = await _service.AdminHideOrDeleteAsync(reviewId, delete);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
