using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecommerce.DataAccess.Services.DebrisAlert;
using Ecommerce.Entities.DTO.DebrisTracking;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DebrisAlertController : ControllerBase
    {
        private readonly IDebrisAlertService _debrisAlertService;
        private readonly ResponseHandler _responseHandler;

        public DebrisAlertController(IDebrisAlertService debrisAlertService, ResponseHandler responseHandler)
        {
            _debrisAlertService = debrisAlertService;
            _responseHandler = responseHandler;
        }

        /// <summary>
        /// Register a satellite for the current user
        /// </summary>
        [HttpPost("register-satellite")]
        public async Task<IActionResult> RegisterSatellite([FromBody] RegisterSatelliteRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return StatusCode((int)_responseHandler.Unauthorized<string>("User not authenticated").StatusCode,
                    _responseHandler.Unauthorized<string>("User not authenticated"));

            var response = await _debrisAlertService.RegisterSatelliteAsync(request, userId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Update satellite threshold
        /// </summary>
        [HttpPut("{satelliteId}/threshold")]
        public async Task<IActionResult> UpdateThreshold(Guid satelliteId, [FromBody] UpdateThresholdRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return StatusCode((int)_responseHandler.Unauthorized<string>("User not authenticated").StatusCode,
                    _responseHandler.Unauthorized<string>("User not authenticated"));

            var response = await _debrisAlertService.SetThresholdAsync(satelliteId, request, userId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get alert history for current user
        /// </summary>
        [HttpGet("alerts/history")]
        public async Task<IActionResult> GetAlertHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return StatusCode((int)_responseHandler.Unauthorized<string>("User not authenticated").StatusCode,
                    _responseHandler.Unauthorized<string>("User not authenticated"));

            var response = await _debrisAlertService.GetAlertHistoryAsync(userId);
            return StatusCode((int)response.StatusCode, response);
        }
        /// <summary>
        /// Get all satellites of current user
        /// </summary>
        [HttpGet("my-satellites")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetMySatellites()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return StatusCode((int)_responseHandler.Unauthorized<string>("User not authenticated").StatusCode,
                    _responseHandler.Unauthorized<string>("User not authenticated"));

            var response = await _debrisAlertService.GetMySatellitesAsync(userId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Run debris check manually (optional → mostly handled by Hangfire job)
        /// </summary>
        [HttpPost("check")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RunCheck()
        {
            var response = await _debrisAlertService.RunDebrisCheckAsync(DateTime.UtcNow);
            return StatusCode((int)response.StatusCode, response);
        }

    }
}