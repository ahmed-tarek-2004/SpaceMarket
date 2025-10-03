using Ecommerce.API.Validators.Payment;
using Ecommerce.DataAccess.Services.Notifications;
using Ecommerce.DataAccess.Services.Payment;
using Ecommerce.Entities.DTO.Payment;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Stripe;
using Stripe.V2;
using System;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ResponseHandler _responseHandler;
        private readonly IValidator<PaymentRequest> _paymentRequestValidator;
        private readonly IValidator<HandlePayment> _handelPaymentValidator;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;
        //private readonly IValidator<CreatePaymentIntentRequest> _paymentValidator;
        //private readonly IValidator<CashOnDeliveryRequest> _cashOnDeliveryValidator;

        public PaymentController( IPaymentService paymentService,ResponseHandler responseHandler
            ,IValidator<PaymentRequest> paymentRequestValidator
            ,IValidator<HandlePayment> handelPaymentValidator
            ,IConfiguration configuration,
            //IValidator<CashOnDeliveryRequest> cashOnDeliveryValidator
            INotificationService notificationService
            )
        {
            _paymentService = paymentService;
            _responseHandler = responseHandler;
            //_checkoutValidator = checkoutValidator;
            _paymentRequestValidator = paymentRequestValidator;
            _handelPaymentValidator = handelPaymentValidator;
            _config= configuration;
            _notificationService = notificationService;
            //_cashOnDeliveryValidator = cashOnDeliveryValidator;
        }
        [HttpPost("checkout-session")]
        [Authorize(Roles ="Client")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] PaymentRequest request)
        {
            var validation = await _paymentRequestValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                    _responseHandler.BadRequest<object>(errors));
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _paymentService.CheckoutSessionService(userId,request);
            return StatusCode((int)response.StatusCode, response);
        }

        //[HttpPost("handle-client-state")]
        //[Authorize(Roles ="Client")]
        //public async Task<IActionResult> HandleState([FromBody] HandlePayment request)
        //{
        //    var validation = await _handelPaymentValidator.ValidateAsync(request);
        //    if (!validation.IsValid)
        //    {
        //        var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
        //        return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
        //            _responseHandler.BadRequest<object>(errors));
        //    }

        //    var clientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var response = await _paymentService.HandlePayment(clientId,request);
        //    return StatusCode((int)response.StatusCode, response);
        //}

        [HttpGet("get/provider/orders-status")]
        [Authorize(Roles = "ServiceProvider")]
        public async Task<IActionResult> GetProviderPaymentAsync()
        {
            var providerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _paymentService.GetProviderOrdersStatusAsync(providerId);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpGet("get/admin/transactions")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminAllTransactionsAsync([FromQuery] TransactionStatus? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _paymentService.GetAdminAllTransactionsAsync(adminId!,status, from, to);
            return StatusCode((int)response.StatusCode, response);
        }

        #region WebHook
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleWebhook()
        {

            var signature = Request.Headers["Stripe-Signature"];

            using var reader = new StreamReader(HttpContext.Request.Body);
            var json = await reader.ReadToEndAsync();
            var response = await _paymentService.HandleWebhookAsync(json, signature);
            return StatusCode((int)response.StatusCode, response);
        }
        #endregion
    }
}
