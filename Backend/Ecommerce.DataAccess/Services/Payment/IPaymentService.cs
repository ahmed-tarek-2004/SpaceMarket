using Ecommerce.Entities.DTO.Payment;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.DataAccess.Services.Payment
{
    public interface IPaymentService
    {
        public Task<Response<PaymentResponse>> CheckoutSessionService(string userEmail,PaymentRequest request);
        public Task <Response<string>>HandlePayment(string clientId,HandlePayment handle);
        public Task<Response<List<ProviderGetOrdersDto>>> GetProviderOrdersAsync(string providerId);
        public Task<Response<object>> HandleWebhookAsync(string json, string stripeSignature);
        public Task<Response<List<TransactionResponse>>> GetAdminAllTransactionsAsync(TransactionStatus? status,DateTime? from,DateTime? to);

    }
}
