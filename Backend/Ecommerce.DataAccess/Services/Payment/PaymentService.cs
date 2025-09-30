using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.Email;
using Ecommerce.Entities.DTO.Payment;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Models.Auth.Users;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Configurations;
using Ecommerce.Utilities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stripe.V2;
using Stripe.V2.MoneyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ecommerce.DataAccess.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ResponseHandler _responseHandler;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly StripeSettings stripe;
        public PaymentService(ILogger<PaymentService> logger, UserManager<User> userManager, ApplicationDbContext context
            , ResponseHandler responseHandler, IOptions<StripeSettings> options, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _responseHandler = responseHandler;
            stripe = options.Value;
            _emailService = emailService;
            //StripeConfiguration.ApiKey = stripe.SecretKey;
        }

        #region checkOut
        public async Task<Response<PaymentResponse>> CheckoutSessionService(string userId, PaymentRequest request)
        {
           
            _logger.LogInformation("Start using SessionCheckout");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            try
            {
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(request.ServiceUnitAmount * 100),
                            Currency = request.Currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = request.ServiceName,
                            }
                        },
                        Quantity = request.Quantity,
                    }
                },
                    Mode = "payment",
                    SuccessUrl = request.SuccessUrl,
                    CancelUrl = request.CancelUrl,
                    CustomerEmail = user.Email,
                    Metadata = new Dictionary<string, string>
                    {
                        { "orderId", request.OrderId.ToString() },
                        { "clientId", user.Id }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);
                _logger.LogInformation(
                  "Checkout session created successfully. SessionId: {SessionId}, Url: {Url}",
                  session.Id,
                  session.Url);

                return _responseHandler.Success(
                    new PaymentResponse
                    {
                        SessionId = session.Id,
                        SessionUrl = session.Url
                    },
                    $"Checkout session created successfully check it by going to {session.SuccessUrl}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkout session");
                return _responseHandler.ServerError<PaymentResponse>("Failed to create checkout session.");
            }
        }

        #endregion
        #region HandlePayment
        //public async Task<Response<string>> HandlePayment(string clientId, HandlePayment handle)
        //{
        //    try
        //    {
        //        var orders = await _context.Orders
        //            .Include(o => o.Items)
        //            .ThenInclude(i => i.Service)
        //            .ThenInclude(s => s.Provider)
        //            .Where(o => o.Id == handle.OrderId)
        //            .FirstOrDefaultAsync();


        //        if (orders == null)
        //        {
        //            return _responseHandler.NotFound<string>($"Order Not Found");
        //        }
        //        var user = await _userManager.Users.FirstOrDefaultAsync(b => b.Id == clientId);

        //        var service = new SessionService();
        //        var session = await service.GetAsync(handle.SessionId);
        //        _logger.LogInformation($"Getting Session Info With Stata{session.PaymentStatus}");

        //        var transaction = new Entities.Models.Transaction
        //        {
        //            Id = Guid.NewGuid(),
        //            OrderId = handle.OrderId,
        //            Status = session.PaymentStatus == "paid" ? TransactionStatus.Paid : TransactionStatus.Pending,
        //            Amount = session.AmountTotal.HasValue ? session.AmountTotal.Value / 100m : 0,
        //            Date = DateTime.UtcNow
        //        };

        //        orders.Status = session.PaymentStatus == "paid" ? OrderStatus.Paid : OrderStatus.Failed;
        //        await _context.Transactions.AddAsync(transaction);
        //        await _context.SaveChangesAsync();
        //        _logger.LogInformation($"Handeled Successfully");

        //        if (session.PaymentStatus == "paid")
        //        {
        //            _logger.LogInformation($"ClientId: {clientId}, OrderId: {handle.OrderId}, SessionId: {handle.SessionId}");
        //            _logger.LogInformation($"User: {(user == null ? "null" : user.Email)}");
        //            _logger.LogInformation($"Session: {(session == null ? "null" : session.Id)}");


        //            //var receiptUrl = await GetReceiptUrl(session.PaymentIntentId);

        //            await _emailService.SendPaymentReceiptAsync(
        //                              user, transaction,
        //                              session
        //                           //,receiptUrl ?? string.Empty
        //                           );

        //        }
        //        return _responseHandler.Success("Success Transaction With Updating Status", "");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating checkout session");
        //        return _responseHandler.ServerError<string>("Failed to Handle Checkout.");
        //    }
        //}
        //private async Task<string> GetReceiptUrl(string paymentIntentId)
        //{

        //    var paymentIntentService = new PaymentIntentService();
        //    var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);

        //    if (paymentIntent.LatestCharge != null)
        //    {
        //        var chargeService = new ChargeService();
        //        var charge = await chargeService.GetAsync(paymentIntent.LatestCharge.ToString());

        //        return charge.ReceiptUrl;
        //    }

        //    return "#";
        //}
        #endregion
        public async Task<Response<List<ProviderGetOrdersDto>>> GetProviderOrdersStatusAsync(string providerId)
        {
            try
            {
                var response = await _context.Orders
                    .Where(o => o.Item.Service.ProviderId == providerId)
                    .Select(o => new ProviderGetOrdersDto
                    {
                        OrderId = o.Id,
                        ClientName = o.Client != null ? o.Client.FullName : "Unknown Client",
                        ClientId = o.ClientId,
                        Amount = o.Amount * o.Commission,
                        PaymentStatus = o.Status,
                        CreatedAt = o.CreatedAt
                    }).ToListAsync();

                var auditLog = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = providerId,
                    EntityName = "Orders",
                    EntityId = Guid.NewGuid(),
                    Details = $"Provider with ProviderId : {providerId} Retrieved about All Orders Status",
                    Action = "Read"

                };
                await _context.AddAsync(auditLog);
                await _context.SaveChangesAsync();

                if (!response.Any())
                    return _responseHandler.NotFound<List<ProviderGetOrdersDto>>("No orders found for this provider.");

                return _responseHandler.Success(response, "Orders retrieved successfully with payment details.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error When Retrieving Provider's TransAction.");
                return _responseHandler.ServerError<List<ProviderGetOrdersDto>>("Failed to Retrieve Transactions.");

            }
        }

        public async Task<Response<List<TransactionResponse>>> GetAdminAllTransactionsAsync(string adminId, TransactionStatus? status, DateTime? from, DateTime? to)
        {
            try
            {
                var transactions = _context.Transactions
                                           .AsNoTracking()
                                           .AsQueryable();

                if (status.HasValue)
                    transactions = transactions.Where(t => t.Status == status);

                if (from.HasValue)
                    transactions = transactions.Where(t => t.Date >= from.Value);

                if (to.HasValue)
                    transactions = transactions.Where(t => t.Date <= to.Value);

                var response = await transactions
                    .Select(t => new TransactionResponse
                    {
                        TransactionId = t.Id,
                        ClientName = t.Client.FullName,
                        ProviderName = t.ServiceProvider.CompanyName,
                        Status = t.Status,
                        Date = t.Date,
                        Amount = t.Amount
                    })
                    .ToListAsync();
                var auditLog = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = adminId,
                    EntityName = "Transactions",
                    EntityId = transactions.FirstOrDefault()?.Id ?? Guid.NewGuid(),
                    Details = $"Admin with adminId : {adminId} Retrieved about All Transactions",
                    Action = "Read"

                };
                await _context.AddAsync(auditLog);
                await _context.SaveChangesAsync();

                return _responseHandler.Success(response, "Admin Got All Transactions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when try to Retrieve Transactions.");
                return _responseHandler.ServerError<List<TransactionResponse>>("Failed to Retrieve Transactions.");
            }
        }

        #region WeebHook
        public async Task<Response<object>> HandleWebhookAsync(string json, string stripeSignature)
        {
            _logger.LogInformation(
                "Received webhook event. Signature: {Signature}",
                stripeSignature);

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    stripe.WebhookSecret,
                    throwOnApiVersionMismatch: false);
                try
                {
                    stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, stripe.WebhookSecret);
                }
                catch (StripeException ex)
                {
                    return _responseHandler.BadRequest<object>(ex.Message);
                }

                _logger.LogInformation(
                    "Stripe Event received: {EventType}",
                    stripeEvent.Type);


                //if (stripeEvent.Type == "checkout.session.completed")
                //{
                var session = stripeEvent.Data.Object as Session;
                if (session == null)
                    return _responseHandler.BadRequest<object>("Event data object is not a session.");

                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id.ToString() == session.Metadata["orderId"]);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == session.Metadata["clientId"]);


                var transaction = new Entities.Models.Transaction
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Date = DateTime.UtcNow
                };

                if (stripeEvent.Type == "checkout.session.completed")
                {

                    order.Status = OrderStatus.Paid;
                    transaction.Status = TransactionStatus.Paid;
                    transaction.Amount = session.AmountTotal.HasValue ? session.AmountTotal.Value / 100m : 0;
                }
                else if (stripeEvent.Type == "payment_intent.payment_failed")
                {
                    order.Status = OrderStatus.Failed;
                    transaction.Status = TransactionStatus.Pending;
                    transaction.Amount = 0;

                }
                else
                {
                    order.Status = OrderStatus.PendingPayment;
                    transaction.Status = TransactionStatus.Pending;
                    transaction.Amount = 0;
                }

                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();


                if (session.PaymentStatus == "paid")
                {
                    _logger.LogInformation(
                        $"Sending payment receipt. ClientId: {user.Id}, OrderId: {order.Id}, SessionId: {session.Id}");

                    await _emailService.SendPaymentReceiptAsync(user, transaction, session);
                }

                return _responseHandler.Success<object>(
                "",
                "Webhook handled successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling webhook");
                return _responseHandler.ServerError<object>("Webhook handling failed.");
            }
        }
        #endregion

    }
}
